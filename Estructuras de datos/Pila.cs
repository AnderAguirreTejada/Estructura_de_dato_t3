using System;

namespace TowerDefenseWPF.EstructurasDeDatos;
public class Pila<T>
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

    private Nodo? cima = null;
    private int cantidad = 0;

    public int Cantidad => cantidad;

    public bool PuedeDeshacerse => cima != null;

    public void Apilar(T dato)
    {
        Nodo nuevo = new Nodo(dato);
        if (cima == null)
        {
            cima = nuevo;
        }
        else
        {
            nuevo.Siguiente = cima;
            cima = nuevo;
        }
        cantidad++;
    }

    public T Desapilar()
    {
        if (cima == null)
            throw new InvalidOperationException("La pila está vacía.");

        T dato = cima.Dato;
        cima = cima.Siguiente;
        cantidad--;
        return dato;
    }

    public bool IntentarDesapilar(out T resultado)
    {
        if (cima == null)
        {
            resultado = default!;
            return false;
        }
        resultado = Desapilar();
        return true;
    }

    public T VerCima()
    {
        if (cima == null)
            throw new InvalidOperationException("La pila está vacía.");
        return cima.Dato;
    }

    public bool IntentarVerCima(out T resultado)
    {
        if (cima == null)
        {
            resultado = default!;
            return false;
        }
        resultado = cima.Dato;
        return true;
    }

    public void Limpiar()
    {
        cima = null;
        cantidad = 0;
    }
}
