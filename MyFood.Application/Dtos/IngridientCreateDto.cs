using System;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngridientCreateDto
{
[MaxLength(260)]
[Required]    
public string Name {get; set;} =string.Empty;
[Required]
public int Quantity {get; set;}
[Required]
public int FoodId{get; set;}
}
