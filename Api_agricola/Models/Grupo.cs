using System;
using System.Collections.Generic;

namespace Api_agricola.Models;

public partial class Grupo
{
    public int Id { get; set; }

    public int? IdLote { get; set; }

    public string Nombre { get; set; } = null!;

}
