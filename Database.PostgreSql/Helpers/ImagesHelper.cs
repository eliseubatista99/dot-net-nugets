namespace Database.PostgreSql.Helpers
{
    public static class ImagesHelper
    {
        public static string? ToBase64DataUri(byte[]? imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            var mimeType = GetMimeType(imageBytes);
            var base64 = Convert.ToBase64String(imageBytes);

            return $"data:{mimeType};base64,{base64}";
        }

        private static string GetMimeType(byte[] bytes)
        {
            // PNG
            if (bytes.Length > 4 &&
                bytes[0] == 0x89 &&
                bytes[1] == 0x50 &&
                bytes[2] == 0x4E &&
                bytes[3] == 0x47)
                return "image/png";

            // JPEG / JPG
            if (bytes.Length > 3 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8)
                return "image/jpeg";

            // GIF
            if (bytes.Length > 3 &&
                bytes[0] == 0x47 &&
                bytes[1] == 0x49 &&
                bytes[2] == 0x46)
                return "image/gif";

            // SVG (texto)
            var svg = System.Text.Encoding.UTF8.GetString(bytes);
            if (svg.Contains("<svg"))
                return "image/svg+xml";

            return "application/octet-stream";
        }
    }
}
