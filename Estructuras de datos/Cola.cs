using System;

namespace TowerDefenseWPF.EstructurasDeDatos;

/// <summary>
/// Recreación personalizada de una cola genérica (FIFO) basada en nodos dinámicos enlazados.
/// Proporciona encolado y desencolado en tiempo constante O(1).
/// </summary>
public class Cola<T>
{
    private class Nodo
    {
        public T Valor;
        public Nodo? Siguiente;

        public Nodo(T valor)
        {
            Valor = valor;
            Siguiente = null;
        }
    }

    private Nodo? _cabeza;
    private Nodo? _cola;
    private int _cantidad;

    public Cola()
    {
        _cabeza = null;
        _cola = null;
        _cantidad = 0;
    }

    public int Count => _cantidad;

    public void Enqueue(T item)
    {
        Nodo nuevoNodo = new Nodo(item);
        if (_cabeza == null)
        {
            _cabeza = nuevoNodo;
            _cola = nuevoNodo;
        }
        else
        {
            _cola!.Siguiente = nuevoNodo;
            _cola = nuevoNodo;
        }
        _cantidad++;
    }

    public T Dequeue()
    {
        if (_cabeza == null)
            throw new InvalidOperationException("La cola está vacía.");

        T valor = _cabeza.Valor;
        _cabeza = _cabeza.Siguiente;
        
        if (_cabeza == null)
        {
            _cola = null;
        }
        
        _cantidad--;
        return valor;
    }

    public bool TryDequeue(out T result)
    {
        if (_cabeza == null)
        {
            result = default!;
            return false;
        }
        result = Dequeue();
        return true;
    }

    public void Clear()
    {
        _cabeza = null;
        _cola = null;
        _cantidad = 0;
    }
}
