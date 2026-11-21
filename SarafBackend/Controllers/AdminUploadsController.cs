using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopMicroservice.Controllers
{
    /// <summary>آپلود عکس محصول (فقط ادمین). فایل داخل wwwroot/uploads ذخیره و آدرس کاملش برگردانده می‌شود.</summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin/uploads")]
    public class AdminUploadsController : ControllerBase
    {
        private const long MaxBytes = 5 * 1024 * 1024; // ۵ مگابایت
        private readonly IWebHostEnvironment _environment;

        public AdminUploadsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        [RequestSizeLimit(MaxBytes + 1024 * 64)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "فایلی انتخاب نشده است." });
            }
            if (file.Length > MaxBytes)
            {
                return BadRequest(new { message = "حجم عکس نباید بیشتر از ۵ مگابایت باشد." });
            }

            // نوع فایل را از «محتوای واقعی» تشخیص می‌دهیم، نه از اسمی که کاربر فرستاده
            var header = new byte[12];
            await using (var probe = file.OpenReadStream())
            {
                var read = await probe.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
                if (read < header.Length)
                {
                    return BadRequest(new { message = "فایل عکس معتبر نیست." });
                }
            }

            var extension = DetectImageExtension(header);
            if (extension is null)
            {
                return BadRequest(new { message = "فقط عکس‌های JPG، PNG و WEBP پذیرفته می‌شود." });
            }

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var folder = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(folder);

            // اسم فایل کاملاً تصادفی است (جلوگیری از بازنویسی و حمله‌ی مسیر)
            var fileName = $"{Guid.NewGuid():N}{extension}";
            await using (var target = System.IO.File.Create(Path.Combine(folder, fileName)))
            {
                await file.CopyToAsync(target, cancellationToken);
            }

            var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            return Ok(new { url });
        }

        private static string? DetectImageExtension(byte[] h)
        {
            // JPEG: FF D8 FF
            if (h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF) return ".jpg";
            // PNG: 89 50 4E 47
            if (h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47) return ".png";
            // WEBP: "RIFF" .... "WEBP"
            if (h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46 &&
                h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50) return ".webp";
            return null;
        }
    }
}
