using System;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Recreación personalizada de una lista dinámica genérica basada en arreglos.
/// Proporciona acceso por índice O(1) y redimensionamiento automático.
/// </summary>
public class Lista<T> : IEnumerable<T>
{
    private T[] _elementos;
    private int _tamaño;
    private const int CapacidadDefecto = 4;

    public Lista()
    {
        _elementos = new T[CapacidadDefecto];
        _tamaño = 0;
    }

    public Lista(int capacidad)
    {
        if (capacidad < 0) throw new ArgumentOutOfRangeException(nameof(capacidad));
        _elementos = new T[capacidad];
        _tamaño = 0;
    }

    public int Count => _tamaño;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _tamaño)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _elementos[index];
        }
        set
        {
            if (index < 0 || index >= _tamaño)
                throw new ArgumentOutOfRangeException(nameof(index));
            _elementos[index] = value;
        }
    }

    public void Add(T item)
    {
        if (_tamaño == _elementos.Length)
            Agrandar();
        _elementos[_tamaño++] = item;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _tamaño)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        _tamaño--;
        if (index < _tamaño)
        {
            // Desplazar elementos hacia la izquierda para llenar el vacío
            Array.Copy(_elementos, index + 1, _elementos, index, _tamaño - index);
        }
        _elementos[_tamaño] = default!; // Liberar referencia
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < _tamaño; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_elementos[i], item))
                return i;
        }
        return -1;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

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

    // Soporte para bucles foreach
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _tamaño; i++)
        {
            yield return _elementos[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
