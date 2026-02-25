// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel.DataAnnotations;

namespace Journal.Database.Models;

public enum Weather
{
    [Display(Name = "Unknown")]
    Unknown,
    [Display(Name = "Sunny")]
    Sunny,
    [Display(Name = "Partly Cloudy")]
    PartlyCloudy,
    [Display(Name = "Cloudy")]
    Cloudy,
    [Display(Name = "Rainy")]
    Rainy,
    [Display(Name = "Snowy")]
    Snowy,
    [Display(Name = "Sleeting")]
    Sleet,
    [Display(Name = "Hailing")]
    Hail,
    [Display(Name = "Stormy")]
    Stormy,
    [Display(Name = "Windy")]
    Windy,
    [Display(Name = "Foggy")]
    Foggy
}
