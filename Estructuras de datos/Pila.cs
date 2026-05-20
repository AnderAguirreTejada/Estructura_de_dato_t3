using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Recreación personalizada de una pila genérica (LIFO).
/// Diseñada desde cero para gestionar el historial de acciones y posibilitar el "Deshacer".
/// </summary>
public class Pila<T>
{
    private T[] _elementos;
    private int _tamaño;
    private const int CapacidadDefecto = 4;

    public Pila()
    {
        _elementos = new T[CapacidadDefecto];
        _tamaño = 0;
    }

    public int Count => _tamaño;
    public bool PuedeDeshacerse => _tamaño > 0; // Propiedad adaptada para HistorialAcciones

    public void Push(T item)
    {
        if (_tamaño == _elementos.Length)
            Agrandar();
        _elementos[_tamaño++] = item;
    }

    public T Pop()
    {
        if (_tamaño == 0)
            throw new InvalidOperationException("La pila está vacía.");
        
        T item = _elementos[--_tamaño];
        _elementos[_tamaño] = default!; // Liberar referencia para el GC
        return item;
    }

    public bool TryPop(out T result)
    {
        if (_tamaño == 0)
        {
            result = default!;
            return false;
        }
        result = Pop();
        return true;
    }

    public bool TryPeek(out T result)
    {
        if (_tamaño == 0)
        {
            result = default!;
            return false;
        }
        result = _elementos[_tamaño - 1];
        return true;
    }

    public void Clear()
    {
        Array.Clear(_elementos, 0, _tamaño);
        _tamaño = 0;
    }

    private void Agrandar()
    {
        int nuevaCapacidad = _elementos.Length == 0 ? CapacidadDefecto : _elementos.Length * 2;
        T[] nuevoArreglo = new T[nuevaCapacidad];
        Array.Copy(_elementos, nuevoArreglo, _tamaño);
        _elementos = nuevoArreglo;
    }
}
