﻿namespace JWTAuthentication.ViewModels;

public class ResetPasswordViewModel
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string Code { get; set; } = default!;
}