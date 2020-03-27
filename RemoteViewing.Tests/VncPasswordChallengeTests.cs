using RemoteViewing.Vnc;
using System.Text;
using Xunit;

namespace RemoteViewing.Tests
{
    public class VncPasswordChallengeTests
    {
        [Fact]
        public void GetChallengeResponseTest()
        {
            VncPasswordChallenge challenger = new VncPasswordChallenge();

            var challenge = new byte[]
            {
                0x71, 0x43,0x19, 0xf2, 0xb3, 0xf6, 0xac, 0xcf,
                0x8c, 0x10, 0xc0, 0x06, 0x6e, 0x73, 0xb1, 0xd9,
            };

            var password = "secret_password".ToCharArray();
            var passwordBytes = VncStream.EncodeString(password, 0, password.Length);
            var response = new byte[16];

            challenger.GetChallengeResponse(challenge, passwordBytes, response);

            var expectedResponse = new byte[]
            {
                0x71, 0xb3, 0x6f, 0xa2, 0x44, 0x5a, 0xee, 0x4f,
                0x08, 0x70, 0x21, 0x69, 0x6e, 0x32, 0x87, 0x8e,
            };

            Assert.Equal(expectedResponse, response);
        }
    }
}
