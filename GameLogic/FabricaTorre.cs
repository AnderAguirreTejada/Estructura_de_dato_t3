using System.Windows;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

public static class FabricaTorre
{
    public static int CostoDe(TipoTorre tipo) => tipo switch
    {
        TipoTorre.Arquero => 50,
        TipoTorre.Cañon => 100,
        TipoTorre.Mago => 80,
        _ => 0
    };

    public static Torre Crear(TipoTorre tipo, Point posicion)
    {
        Torre torre = tipo switch
        {
            TipoTorre.Arquero => new Torre
            {
                Tipo = tipo,
                Posicion = posicion,
                Rango = 135,
                Daño = 14,
                VelocidadDisparo = 2.0,
                VelocidadProyectil = 420
            },
            TipoTorre.Cañon => new Torre
            {
                Tipo = tipo,
                Posicion = posicion,
                Rango = 115,
                Daño = 38,
                VelocidadDisparo = 0.6,
                VelocidadProyectil = 280,
                EsExplosivo = true,
                RadioExplosion = 48
            },
            TipoTorre.Mago => new Torre
            {
                Tipo = tipo,
                Posicion = posicion,
                Rango = 150,
                Daño = 20,
                VelocidadDisparo = 1.2,
                VelocidadProyectil = 340,
                CantidadRalenti = 0.4,
                DuracionRalenti = 1.4
            },
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };

        torre.OroTotalInvertido = CostoDe(tipo);
        torre.RaizMejora = ArbolMejoras.ConstruirPara(tipo);
        torre.NodoActual = torre.RaizMejora;
        return torre;
    }
}
