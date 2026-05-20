namespace TowerDefenseWPF.Models;

/// <summary>
/// Una instrucción de spawn: qué enemigo y cuánto esperar antes de soltarlo.
/// Se encola en una Queue<GeneracionEnemigo> para gestionar el orden FIFO de la oleada.
/// </summary>
public record GeneracionEnemigo(TipoEnemigo Tipo, double EsperaAntes);
