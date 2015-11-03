using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Player
{
    public class EditPlayerViewModel
    {
        [RequiredNO]
        public Guid Id { get; set; }
        [RequiredNO]
        [Display(Name = "Fornavn")]
        public string FirstName { get; set; }
        [Display(Name = "Mellomnavn")]
        public string MiddleName { get; set; }
        [RequiredNO]
        [Display(Name = "Etternavn")]
        public string LastName { get; set; }
        public string Fullname => $"{FirstName} {MiddleName} {LastName}";
        public string ImageFull { get; set; }
        [RequiredNO]
        [Display(Name = Res.BirthDate)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = Res.StartDate)]
        [RequiredNO]
        public DateTime StartDate { get; set; }
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Vennligst skriv inn et gyldig telefonnummer")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = Res.Phone)]
        [RequiredNO]
        public int Phone { get; set; }

        [Display(Name = Res.Positions)]
        public IEnumerable<Position> Positions { get; set; }

        public SelectList AllPositions => new SelectList(Enum.GetValues(typeof(Position)).Cast<Position>().Select(v => new SelectListItem
        {
            Text = v.ToString(),
            Value = ((int)v).ToString()
        }).ToList(), "Value", "Text");


        public EditPlayerViewModel()
        {
            
        }

        public EditPlayerViewModel(Models.Domain.Player player)
        {
            Id = player.Id;
            FirstName = player.FirstName;
            MiddleName = player.MiddleName;
            LastName = player.LastName;
            ImageFull = player.ImageFull;
            Phone = player.Phone;
            StartDate = player.StartDate;
            BirthDate = player.BirthDate;
            Positions = player.Positions;
        }

    }
}