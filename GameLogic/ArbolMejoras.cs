using TowerDefenseWPF.EstructurasDeDatos;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Construye el ÁRBOL BINARIO de mejoras para cada tipo de torre.
///
/// Estructura: ÁRBOL BINARIO (máximo 2 hijos por nodo).
/// La raíz representa el estado base de la torre. Cada nodo tiene:
/// - Izquierda: una especialización específica
/// - Derecha: otra especialización alternativa
///
/// El sistema utiliza ArbolBinario<NodoMejora> de las estructuras personalizadas,
/// sin usar estructuras predefinidas de C#.
/// </summary>
public static class ArbolMejoras
{
    public static ArbolBinario<NodoMejora> ConstruirPara(TipoTorre tipo) => tipo switch
    {
        TipoTorre.Arquero => ConstruirArquero(),
        TipoTorre.Cañon => ConstruirCañon(),
        TipoTorre.Mago => ConstruirMago(),
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    private static ArbolBinario<NodoMejora> ConstruirArquero()
    {
        var arbol = new ArbolBinario<NodoMejora>();

        // Raíz
        var raiz = new NodoMejora
        {
            Nombre = "Arquero Base",
            Descripcion = "Sin mejoras",
            Nivel = 0,
            Orden = 0
        };

        // Nivel 1: Izquierda = Daño (Orden negativo), Derecha = Velocidad (Orden positivo)
        var dmg1 = new NodoMejora
        {
            Nombre = "+Daño",
            Descripcion = "+8 daño",
            Costo = 50,
            Nivel = 1,
            Orden = -10,
            Aplicar = t => t.Daño += 8,
            Revertir = t => t.Daño -= 8
        };

        var spd1 = new NodoMejora
        {
            Nombre = "+Velocidad",
            Descripcion = "Ritmo x1.5",
            Costo = 60,
            Nivel = 1,
            Orden = 10,
            Aplicar = t => t.VelocidadDisparo *= 1.5,
            Revertir = t => t.VelocidadDisparo /= 1.5
        };

        // Nivel 2 para rama Daño
        var dmg2a = new NodoMejora
        {
            Nombre = "++Daño",
            Descripcion = "+15 daño",
            Costo = 100,
            Nivel = 2,
            Orden = -20,
            Aplicar = t => t.Daño += 15,
            Revertir = t => t.Daño -= 15
        };

        var dmg2b = new NodoMejora
        {
            Nombre = "Tiro Certero",
            Descripcion = "+6 daño, +25 rango",
            Costo = 110,
            Nivel = 2,
            Orden = -5,
            Aplicar = t => { t.Daño += 6; t.Rango += 25; },
            Revertir = t => { t.Daño -= 6; t.Rango -= 25; }
        };

        // Nivel 2 para rama Velocidad
        var spd2a = new NodoMejora
        {
            Nombre = "++Velocidad",
            Descripcion = "Ritmo x1.4",
            Costo = 110,
            Nivel = 2,
            Orden = 5,
            Aplicar = t => t.VelocidadDisparo *= 1.4,
            Revertir = t => t.VelocidadDisparo /= 1.4
        };

        var spd2b = new NodoMejora
        {
            Nombre = "Multidisparo",
            Descripcion = "+5 daño, ritmo x1.2",
            Costo = 130,
            Nivel = 2,
            Orden = 20,
            Aplicar = t => { t.Daño += 5; t.VelocidadDisparo *= 1.2; },
            Revertir = t => { t.Daño -= 5; t.VelocidadDisparo /= 1.2; }
        };

        // Insertar en orden para que el árbol binario los coloque automáticamente
        arbol.Insertar(raiz);
        arbol.Insertar(dmg1);
        arbol.Insertar(spd1);
        arbol.Insertar(dmg2a);
        arbol.Insertar(dmg2b);
        arbol.Insertar(spd2a);
        arbol.Insertar(spd2b);

        return arbol;
    }

    private static ArbolBinario<NodoMejora> ConstruirCañon()
    {
        var arbol = new ArbolBinario<NodoMejora>();

        var raiz = new NodoMejora
        {
            Nombre = "Cañón Base",
            Descripcion = "Sin mejoras",
            Nivel = 0,
            Orden = 0
        };

        var dmg1 = new NodoMejora
        {
            Nombre = "+Daño",
            Descripcion = "+25 daño",
            Costo = 90,
            Nivel = 1,
            Orden = -10,
            Aplicar = t => t.Daño += 25,
            Revertir = t => t.Daño -= 25
        };

        var area1 = new NodoMejora
        {
            Nombre = "+Área",
            Descripcion = "+20 radio explosión",
            Costo = 90,
            Nivel = 1,
            Orden = 10,
            Aplicar = t => t.RadioExplosion += 20,
            Revertir = t => t.RadioExplosion -= 20
        };

        var dmg2a = new NodoMejora
        {
            Nombre = "Munición Pesada",
            Descripcion = "+35 daño",
            Costo = 140,
            Nivel = 2,
            Orden = -20,
            Aplicar = t => t.Daño += 35,
            Revertir = t => t.Daño -= 35
        };

        var dmg2b = new NodoMejora
        {
            Nombre = "Explosivo",
            Descripcion = "+20 daño, +15 radio",
            Costo = 150,
            Nivel = 2,
            Orden = -5,
            Aplicar = t => { t.Daño += 20; t.RadioExplosion += 15; },
            Revertir = t => { t.Daño -= 20; t.RadioExplosion -= 15; }
        };

        var area2a = new NodoMejora
        {
            Nombre = "Onda Expansiva",
            Descripcion = "+25 radio",
            Costo = 140,
            Nivel = 2,
            Orden = 5,
            Aplicar = t => t.RadioExplosion += 25,
            Revertir = t => t.RadioExplosion -= 25
        };

        var area2b = new NodoMejora
        {
            Nombre = "Recarga Rápida",
            Descripcion = "Ritmo x1.5",
            Costo = 140,
            Nivel = 2,
            Orden = 20,
            Aplicar = t => t.VelocidadDisparo *= 1.5,
            Revertir = t => t.VelocidadDisparo /= 1.5
        };

        arbol.Insertar(raiz);
        arbol.Insertar(dmg1);
        arbol.Insertar(area1);
        arbol.Insertar(dmg2a);
        arbol.Insertar(dmg2b);
        arbol.Insertar(area2a);
        arbol.Insertar(area2b);

        return arbol;
    }

    private static ArbolBinario<NodoMejora> ConstruirMago()
    {
        var arbol = new ArbolBinario<NodoMejora>();

        var raiz = new NodoMejora
        {
            Nombre = "Mago Base",
            Descripcion = "Sin mejoras",
            Nivel = 0,
            Orden = 0
        };

        var ice1 = new NodoMejora
        {
            Nombre = "+Hielo",
            Descripcion = "+15% ralent., +0.5s",
            Costo = 60,
            Nivel = 1,
            Orden = -10,
            Aplicar = t => { t.CantidadRalenti += 0.15; t.DuracionRalenti += 0.5; },
            Revertir = t => { t.CantidadRalenti -= 0.15; t.DuracionRalenti -= 0.5; }
        };

        var arc1 = new NodoMejora
        {
            Nombre = "+Arcano",
            Descripcion = "+10 daño",
            Costo = 70,
            Nivel = 1,
            Orden = 10,
            Aplicar = t => t.Daño += 10,
            Revertir = t => t.Daño -= 10
        };

        var ice2a = new NodoMejora
        {
            Nombre = "Congelar",
            Descripcion = "+20% ralentización",
            Costo = 130,
            Nivel = 2,
            Orden = -20,
            Aplicar = t => t.CantidadRalenti += 0.2,
            Revertir = t => t.CantidadRalenti -= 0.2
        };

        var ice2b = new NodoMejora
        {
            Nombre = "Ventisca",
            Descripcion = "+10% ralent., +1s",
            Costo = 140,
            Nivel = 2,
            Orden = -5,
            Aplicar = t => { t.CantidadRalenti += 0.1; t.DuracionRalenti += 1.0; },
            Revertir = t => { t.CantidadRalenti -= 0.1; t.DuracionRalenti -= 1.0; }
        };

        var arc2a = new NodoMejora
        {
            Nombre = "Rayo Místico",
            Descripcion = "+15 daño, +20 rango",
            Costo = 130,
            Nivel = 2,
            Orden = 5,
            Aplicar = t => { t.Daño += 15; t.Rango += 20; },
            Revertir = t => { t.Daño -= 15; t.Rango -= 20; }
        };

        var arc2b = new NodoMejora
        {
            Nombre = "Caos",
            Descripcion = "+10 daño, ritmo x1.4",
            Costo = 140,
            Nivel = 2,
            Orden = 20,
            Aplicar = t => { t.Daño += 10; t.VelocidadDisparo *= 1.4; },
            Revertir = t => { t.Daño -= 10; t.VelocidadDisparo /= 1.4; }
        };

        arbol.Insertar(raiz);
        arbol.Insertar(ice1);
        arbol.Insertar(arc1);
        arbol.Insertar(ice2a);
        arbol.Insertar(ice2b);
        arbol.Insertar(arc2a);
        arbol.Insertar(arc2b);

        return arbol;
    }
}
