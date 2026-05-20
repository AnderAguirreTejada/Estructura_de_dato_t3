using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Construye el ÁRBOL de mejoras para cada tipo de torre.
///
/// Estructura clave: ÁRBOL n-ario (NodoMejora con Hijos y Padre).
/// La raíz representa el estado base de la torre y cada hijo representa
/// una rama de especialización. Al mejorar, el jugador navega del nodo
/// actual a uno de sus hijos. Esto fuerza decisiones estratégicas porque
/// las ramas son mutuamente excluyentes.
///
/// Cada nodo guarda referencia a su padre, lo que permite revertir mejoras
/// en combinación con la Pila del historial.
/// </summary>
public static class ArbolMejoras
{
    public static NodoMejora ConstruirPara(TipoTorre tipo) => tipo switch
    {
        TipoTorre.Arquero => ConstruirArquero(),
        TipoTorre.Cañon => ConstruirCañon(),
        TipoTorre.Mago => ConstruirMago(),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    private static NodoMejora ConstruirArquero()
    {
        var raiz = new NodoMejora { Nombre = "Arquero Base", Descripcion = "Sin mejoras", Nivel = 0 };

        var dmg1 = new NodoMejora
        {
            Nombre = "+Daño",
            Descripcion = "+8 daño",
            Costo = 50,
            Nivel = 1,
            Aplicar = t => t.Daño += 8,
            Revertir = t => t.Daño -= 8
        };
        var spd1 = new NodoMejora
        {
            Nombre = "+Velocidad",
            Descripcion = "Ritmo x1.5",
            Costo = 60,
            Nivel = 1,
            Aplicar = t => t.VelocidadDisparo *= 1.5,
            Revertir = t => t.VelocidadDisparo /= 1.5
        };

        dmg1.AgregarHijo(new NodoMejora
        {
            Nombre = "++Daño",
            Descripcion = "+15 daño",
            Costo = 100,
            Nivel = 2,
            Aplicar = t => t.Daño += 15,
            Revertir = t => t.Daño -= 15
        });
        dmg1.AgregarHijo(new NodoMejora
        {
            Nombre = "Tiro Certero",
            Descripcion = "+6 daño, +25 rango",
            Costo = 110,
            Nivel = 2,
            Aplicar = t => { t.Daño += 6; t.Rango += 25; },
            Revertir = t => { t.Daño -= 6; t.Rango -= 25; }
        });

        spd1.AgregarHijo(new NodoMejora
        {
            Nombre = "++Velocidad",
            Descripcion = "Ritmo x1.4",
            Costo = 110,
            Nivel = 2,
            Aplicar = t => t.VelocidadDisparo *= 1.4,
            Revertir = t => t.VelocidadDisparo /= 1.4
        });
        spd1.AgregarHijo(new NodoMejora
        {
            Nombre = "Multidisparo",
            Descripcion = "+5 daño, ritmo x1.2",
            Costo = 130,
            Nivel = 2,
            Aplicar = t => { t.Daño += 5; t.VelocidadDisparo *= 1.2; },
            Revertir = t => { t.Daño -= 5; t.VelocidadDisparo /= 1.2; }
        });

        raiz.AgregarHijo(dmg1);
        raiz.AgregarHijo(spd1);
        return raiz;
    }

    private static NodoMejora ConstruirCañon()
    {
        var raiz = new NodoMejora { Nombre = "Cañón Base", Descripcion = "Sin mejoras", Nivel = 0 };

        var dmg1 = new NodoMejora
        {
            Nombre = "+Daño",
            Descripcion = "+25 daño",
            Costo = 90,
            Nivel = 1,
            Aplicar = t => t.Daño += 25,
            Revertir = t => t.Daño -= 25
        };
        var area1 = new NodoMejora
        {
            Nombre = "+Área",
            Descripcion = "+20 radio explosión",
            Costo = 90,
            Nivel = 1,
            Aplicar = t => t.RadioExplosion += 20,
            Revertir = t => t.RadioExplosion -= 20
        };

        dmg1.AgregarHijo(new NodoMejora
        {
            Nombre = "Munición Pesada",
            Descripcion = "+35 daño",
            Costo = 140,
            Nivel = 2,
            Aplicar = t => t.Daño += 35,
            Revertir = t => t.Daño -= 35
        });
        dmg1.AgregarHijo(new NodoMejora
        {
            Nombre = "Explosivo",
            Descripcion = "+20 daño, +15 radio",
            Costo = 150,
            Nivel = 2,
            Aplicar = t => { t.Daño += 20; t.RadioExplosion += 15; },
            Revertir = t => { t.Daño -= 20; t.RadioExplosion -= 15; }
        });

        area1.AgregarHijo(new NodoMejora
        {
            Nombre = "Onda Expansiva",
            Descripcion = "+25 radio",
            Costo = 140,
            Nivel = 2,
            Aplicar = t => t.RadioExplosion += 25,
            Revertir = t => t.RadioExplosion -= 25
        });
        area1.AgregarHijo(new NodoMejora
        {
            Nombre = "Recarga Rápida",
            Descripcion = "Ritmo x1.5",
            Costo = 140,
            Nivel = 2,
            Aplicar = t => t.VelocidadDisparo *= 1.5,
            Revertir = t => t.VelocidadDisparo /= 1.5
        });

        raiz.AgregarHijo(dmg1);
        raiz.AgregarHijo(area1);
        return raiz;
    }

    private static NodoMejora ConstruirMago()
    {
        var raiz = new NodoMejora { Nombre = "Mago Base", Descripcion = "Sin mejoras", Nivel = 0 };

        var ice1 = new NodoMejora
        {
            Nombre = "+Hielo",
            Descripcion = "+15% ralent., +0.5s",
            Costo = 60,
            Nivel = 1,
            Aplicar = t => { t.CantidadRalenti += 0.15; t.DuracionRalenti += 0.5; },
            Revertir = t => { t.CantidadRalenti -= 0.15; t.DuracionRalenti -= 0.5; }
        };
        var arc1 = new NodoMejora
        {
            Nombre = "+Arcano",
            Descripcion = "+10 daño",
            Costo = 70,
            Nivel = 1,
            Aplicar = t => t.Daño += 10,
            Revertir = t => t.Daño -= 10
        };

        ice1.AgregarHijo(new NodoMejora
        {
            Nombre = "Congelar",
            Descripcion = "+20% ralentización",
            Costo = 130,
            Nivel = 2,
            Aplicar = t => t.CantidadRalenti += 0.2,
            Revertir = t => t.CantidadRalenti -= 0.2
        });
        ice1.AgregarHijo(new NodoMejora
        {
            Nombre = "Ventisca",
            Descripcion = "+10% ralent., +1s",
            Costo = 140,
            Nivel = 2,
            Aplicar = t => { t.CantidadRalenti += 0.1; t.DuracionRalenti += 1.0; },
            Revertir = t => { t.CantidadRalenti -= 0.1; t.DuracionRalenti -= 1.0; }
        });

        arc1.AgregarHijo(new NodoMejora
        {
            Nombre = "Rayo Místico",
            Descripcion = "+15 daño, +20 rango",
            Costo = 130,
            Nivel = 2,
            Aplicar = t => { t.Daño += 15; t.Rango += 20; },
            Revertir = t => { t.Daño -= 15; t.Rango -= 20; }
        });
        arc1.AgregarHijo(new NodoMejora
        {
            Nombre = "Caos",
            Descripcion = "+10 daño, ritmo x1.4",
            Costo = 140,
            Nivel = 2,
            Aplicar = t => { t.Daño += 10; t.VelocidadDisparo *= 1.4; },
            Revertir = t => { t.Daño -= 10; t.VelocidadDisparo /= 1.4; }
        });

        raiz.AgregarHijo(arc1);
        raiz.AgregarHijo(ice1);
        return raiz;
    }
}
