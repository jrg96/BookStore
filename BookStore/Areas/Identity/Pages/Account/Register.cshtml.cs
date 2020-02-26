using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BookStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        
        /*
         * -------------------------------------------------------
         * PROPIEDADES AGREGADAS MANUALMENTE
         * -------------------------------------------------------
         */
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyRepository _companyRepository;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ICompanyRepository companyRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;


            _roleManager = roleManager;
            _companyRepository = companyRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            /*
             * --------------------------------------------------------------
             * AGREGAMOS CUSTOM FIELDS AL MODEL VIEW
             * --------------------------------------------------------------
             */

            [Required]
            public string Name { get; set; }

            public string StreetAddress { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public int? CompanyId { get; set; }

            public string Role { get; set; }

            /*
             * ----------------------------------------------------------------------
             * AGREGAMOS DROPDOWNS DE ROLES DISPONIBLES PRELIMINARMENTE
             * ----------------------------------------------------------------------
             */
            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            /*
             * ----------------------------------------------------------------------
             * AGREGANDO LOS DROPDOWNS AL MODEL VIEW
             * ----------------------------------------------------------------------
             */
            Input = new InputModel()
            {
                CompanyList = (await _companyRepository.GetAllCompanies())
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),

                RoleList = (_roleManager.Roles.Where(u => u.Name != SD.ROLE_USER_INDIVIDUAL))
                    .Select(i => i.Name)
                    .Select(i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                /*
                 * --------------------------------------------------------------
                 * RELLENAMOS CUSTOM APPLICATION USER
                 * --------------------------------------------------------------
                 */
                var user = new ApplicationUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    CompanyId = Input.CompanyId,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    State = Input.State,
                    Name = Input.Name,
                    Role = Input.Role
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    /*
                     * ---------------------------------------------------------------------
                     * CHEQUEAMOS SI LOS ROLES YA SE ENCUENTRAN EN LA DB, SINO LOS CREAMOS
                     * ---------------------------------------------------------------------
                     */
                    if (!await _roleManager.RoleExistsAsync(SD.ROLE_ADMIN))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_ADMIN));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.ROLE_USER_INDIVIDUAL))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_INDIVIDUAL));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.ROLE_USER_COMPANY))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_COMPANY));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.ROLE_EMPLOYEE))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_EMPLOYEE));
                    }


                    // CUIDADO: CON ESTA LINEA CREAMOS UN USUARIO CON ROL ADMINISTRADOR
                    // await _userManager.AddToRoleAsync(user, SD.ROLE_ADMIN);

                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.ROLE_USER_INDIVIDUAL);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.ROLE_USER_COMPANY);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }





                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        /*
                         * -----------------------------------------------------------------
                         * CHEQUEAMOS SI EL ADMINISTRADOR AGREGO A UN USUARIO O NO
                         * -----------------------------------------------------------------
                         */
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User", new { Area="Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
