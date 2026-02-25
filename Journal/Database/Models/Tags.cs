using System.ComponentModel.DataAnnotations;

namespace Journal.Database.Models;

public enum MotivationTag
{
    [Display(Name = "Stayed In Bed")]
    StayedInBed,
    [Display(Name = "Not Motivated but did stuff")]
    NotMotivatedButDidStuff,
    [Display(Name = "Slightly Motivated")]
    SlightlyMotivated,
    [Display(Name = "Motivated")]
    Motivated,
    [Display(Name = "Excited")]
    Excited
}

public enum SocialTag
{
    [Display(Name = "No Social Contact")]
    NoSocialContact,
    [Display(Name = "Slight Social Contact")]
    SlightSocialContact,
    [Display(Name = "Active Social Contact")]
    ActiveSocialContact,
    [Display(Name = "Too much Social Contact")]
    TooMuchSocialContact
}

public enum StressTag
{
    [Display(Name = "No Stress")]
    NoStress,
    [Display(Name = "Low Stress")]
    LowStress,
    [Display(Name = "Moderate Stress")]
    ModerateStress,
    [Display(Name = "High Stress")]
    HighStress
}
