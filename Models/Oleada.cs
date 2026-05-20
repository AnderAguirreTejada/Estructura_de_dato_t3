namespace TowerDefenseWPF.Models;

public class Oleada
{
    public int Numero { get; init; }
    public List<GeneracionEnemigo> Generaciones { get; init; } = new();
    public int BonusRecompensa { get; init; }
}
