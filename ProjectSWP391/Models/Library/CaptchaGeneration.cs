namespace ProjectSWP391.Models.Library
{
    public class CaptchaGeneration
    {
        public static string GenerateCaptcha()
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = 8; // Change this value to set the length of the CAPTCHA

            Random random = new Random();
            char[] captchaArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                captchaArray[i] = characters[random.Next(characters.Length)];
            }

            return new string(captchaArray);
        }
    }
}
