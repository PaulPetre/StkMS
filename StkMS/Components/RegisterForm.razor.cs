using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StkMS.Client.Services.Exceptions;
using StkMS.Client.Services.Interfaces;
using StkMS.Shared.Models;

namespace StkMS.Components
{
    public partial class RegisterForm
    {


        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        private RegisterRequest _model = new();
        private bool _isBusy = false;
        private string _errorMessage = string.Empty;

        private async Task RegisterUserAsync()
        {
            _isBusy = true;
            _errorMessage = string.Empty;

            try
            {
                await AuthenticationService.RegisterUserAsync(_model);
                Navigation.NavigateTo("Login");
            }
            catch (ApiException ex)
            {
                // Hanlde the errors of the aPI 
                // TODO: Log those errors 
                _errorMessage = ex.ApiErrorResponse.Message;
            }
            catch (Exception ex)
            {
                // Handle errors 
                _errorMessage = ex.Message;
            }

            _isBusy = false;
        }

        private void RedirectToLogin()
        {
            if (_model.Email != null && _model.Password != null)
            {
                Navigation.NavigateTo("Login");
            }
        }

    }
}