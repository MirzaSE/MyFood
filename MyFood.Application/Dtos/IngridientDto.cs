using System;

namespace MyFood.Application.Dtos;

public class IngridientDto
{
public int Id {get; set;}
public string? Name {get; set;}
public int Quantity {get; set;}
public int FoodId{get;set;}
}
