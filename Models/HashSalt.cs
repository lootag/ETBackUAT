namespace Models
{
    public class HashSalt
    {
        public HashSalt(byte[] hash, byte[] salt)
        {
            Hash = hash;
            Salt = salt;
        }
        public byte[] Hash { get; }
        public byte[] Salt { get; }
        
    }
}