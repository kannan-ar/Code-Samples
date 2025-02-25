using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Fido2Controller : ControllerBase
    {
        private readonly IFido2 _fido2;

        public Fido2Controller(IFido2 fido2)
        {
            _fido2 = fido2;
        }

        [HttpPost("register/options")]
        public ActionResult RegisterOptions([FromBody] RegisterModel userModel)
        {
            var user = new Fido2User
            {
                Id = Guid.NewGuid().ToByteArray(),
                Name = userModel.UserName,
                DisplayName = userModel.UserName
            };

            var authenticatorSelection = new AuthenticatorSelection
            {
                AuthenticatorAttachment = null,
                ResidentKey = ResidentKeyRequirement.Discouraged,
                UserVerification = UserVerificationRequirement.Preferred
            };

            var attestationPreference = AttestationConveyancePreference.None;

            var extensions = new AuthenticationExtensionsClientInputs
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs { Attestation = "none" },
                CredProps = true
            };

            var options = _fido2.RequestNewCredential(
            user,
            new List<PublicKeyCredentialDescriptor>(),
            authenticatorSelection,
            attestationPreference,
            extensions);

            HttpContext.Session.SetString("Fido2AttestationOptions", options.ToJson());

            return Ok(options);
        }

        [HttpPost("register/complete")]
        public async Task<ActionResult> RegisterComplete([FromBody] AuthenticatorAttestationRawResponse response, [FromQuery] string username)
        {
            var json = HttpContext.Session.GetString("Fido2AttestationOptions");
            var options = CredentialCreateOptions.FromJson(json);

            var result = await _fido2.MakeNewCredentialAsync(response, options, null);
            return Ok(result);
        }
    }
}
