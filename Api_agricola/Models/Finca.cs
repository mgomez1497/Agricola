using System;
using System.Collections.Generic;

namespace Api_agricola.Models;

public partial class Finca
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public decimal Hectareas { get; set; }

    public string? Descripcion { get; set; }
}
