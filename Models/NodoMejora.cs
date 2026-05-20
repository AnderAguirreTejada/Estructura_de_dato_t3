namespace TowerDefenseWPF.Models;

/// <summary>
/// Nodo del árbol de mejoras de una torre.
///
/// Estructura: ÁRBOL n-ario. La raíz representa el estado base de la torre
/// y cada hijo representa una mejora aplicable. Al elegir un hijo, el jugador
/// "desciende" por una rama (especialización). Las ramas no se mezclan, lo que
/// fuerza al jugador a elegir un estilo de mejora.
///
/// Se almacena la referencia al padre para poder revertir mejoras (junto con la Pila).
/// </summary>
public class NodoMejora
{
    public string Nombre { get; init; } = "";
    public string Descripcion { get; init; } = "";
    public int Costo { get; init; }
    public int Nivel { get; init; }

    public Action<Torre> Aplicar { get; init; } = _ => { };
    public Action<Torre> Revertir { get; init; } = _ => { };

    public List<NodoMejora> Hijos { get; } = new();
    public NodoMejora? Padre { get; set; }

    public NodoMejora AgregarHijo(NodoMejora hijo)
    {
        hijo.Padre = this;
        Hijos.Add(hijo);
        return this;
    }
}
