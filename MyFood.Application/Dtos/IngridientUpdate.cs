using System;
using System.ComponentModel.DataAnnotations;

namespace MyFood.Application.Dtos;

public class IngridientUpdate
{
    [MaxLength(260)]
public string? Name{get;set;}
public int Quantity{get;set;}
public int? FoodId{get;set;}

}
