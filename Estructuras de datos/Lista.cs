using System;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefenseWPF.EstructurasDeDatos;
public class Lista<T> : IEnumerable<T>
{
    private class Nodo
    {
        public T Dato;
        public Nodo? Siguiente;

        public Nodo(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }

    private Nodo? primero = null;
    private Nodo? ultimo = null;
    private int cantidad = 0;

    public int Cantidad => cantidad;

    public bool EstaVacia => primero == null;

    public void Agregar(T dato)
    {
        Nodo nuevo = new Nodo(dato);
        if (primero == null)
        {
            primero = nuevo;
            ultimo = nuevo;
        }
        else
        {
            ultimo!.Siguiente = nuevo;
            ultimo = nuevo;
        }
        cantidad++;
    }

    public bool Eliminar(T dato)
    {
        Nodo? actual = primero;
        Nodo? anterior = null;

        while (actual != null)
        {
            if (EqualityComparer<T>.Default.Equals(actual.Dato, dato))
            {
                if (anterior == null)
                {
                    primero = actual.Siguiente;
                }
                else
                {
                    anterior.Siguiente = actual.Siguiente;
                }

                if (actual.Siguiente == null)
                {
                    ultimo = anterior;
                }

                cantidad--;
                return true;
            }

            anterior = actual;
            actual = actual.Siguiente;
        }

        return false;
    }

    public bool Contiene(T dato)
    {
        Nodo? temp = primero;
        while (temp != null)
        {
            if (EqualityComparer<T>.Default.Equals(temp.Dato, dato))
                return true;
            temp = temp.Siguiente;
        }
        return false;
    }

    public T this[int indice]
    {
        get
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(indice));

            Nodo? temp = primero;
            for (int i = 0; i < indice; i++)
                temp = temp!.Siguiente;

            return temp!.Dato;
        }
    }

    public void Limpiar()
    {
        primero = null;
        ultimo = null;
        cantidad = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Nodo? temp = primero;
        while (temp != null)
        {
            yield return temp.Dato;
            temp = temp.Siguiente;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
