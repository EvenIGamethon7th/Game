// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UuBjQFJvZGtI5CrklW9jY2NnYmHLyYHD7xDoKIiQvSFZiMHmrRjkbuBjbWJS4GNoYOBjY2KEH1DuF9X/q2cD0lbCqKutiDOOKnX6stxSYwnfrAyLFJR6Y4imJCkv0qbGpp4EaTfyjDSyAbJJYKlhVYrlWZPEuDs7/7xOhvJlN6BDhZEECVaONAkE3vpSgj1Q40+Fvq0Gf6679bKGpWZ3Eia/ACkUPfJed2gIHIhf0KOcIP+GjuuqCfsZJd6Qksk8e9VQfSX5DBNqdAevIHyRv/9IAB9kT2CHlt6CfHiSS5XW4ggBRsVhsIIXCqSzKG6f6QTKFyRTKC5zKUQiAEz/vAFQwhQT2c6WFuVUVCWlFZja7abTFTe62MZbz2k6rdWf/WBhY2Jj");
        private static int[] order = new int[] { 0,5,5,6,7,9,12,7,8,9,11,13,13,13,14 };
        private static int key = 98;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
