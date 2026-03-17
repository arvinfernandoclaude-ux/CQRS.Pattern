namespace CQRS.Pattern.Domain.Entities;

public class AspNetUser
{
    public Guid Id { get; set; }
    public string? UserName { get; private set; }
    public string? NormalizedUserName { get; private set; }
    public string? Email { get; private set; }
    public string? NormalizedEmail { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public string? PasswordHash { get; private set; }
    public string? SecurityStamp { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public DateTimeOffset? LockoutEnd { get; private set; }
    public bool LockoutEnabled { get; private set; }
    public int AccessFailedCount { get; private set; }

    public void SetUserName(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        if (userName.Length > 256)
            throw new ArgumentException("UserName cannot exceed 256 characters.", nameof(userName));

        UserName = userName;
        NormalizedUserName = userName.ToUpperInvariant();
    }

    public void SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (email.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters.", nameof(email));

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void EnableTwoFactor()
    {
        TwoFactorEnabled = true;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
    }

    public void EnableLockout(DateTimeOffset lockoutEnd)
    {
        LockoutEnabled = true;
        LockoutEnd = lockoutEnd;
    }

    public void DisableLockout()
    {
        LockoutEnabled = false;
        LockoutEnd = null;
        AccessFailedCount = 0;
    }

    public void IncrementAccessFailedCount()
    {
        AccessFailedCount++;
    }

    public void ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
    }
}
