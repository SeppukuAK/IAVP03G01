using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Priority_Queue;


public class Nodo
{
    private Nodo _padre;

    private Pos _pos;

    private int _f; //Coste desde el inicio a este nodo
    private int _g; //Coste desde este nodo al nodo objetivo

    public Nodo(Nodo padre, Pos pos)
    {
        _padre = padre;
        _pos = pos;
        _f = _g = 0;
    }

    //---------------------------- GETTERS ----------------------------------------------
    public Nodo getPadre()
    {
        return _padre;
    }
		
	public Pos GetPos()
	{
		return _pos;
	}

	public int GetF()
	{
		return _f;
	}
	public int GetG()
	{
		return _g;
	}

    //---------------------------- SETTERS ----------------------------------------------
    public void SetF(int f)
	{
		_f = f;
	}
	public void SetG(int g)
	{
		_g = g;
	}
}

public class AEstrella
{
    //Atributos

    //Empty si no hay camino posible
    public Stack<Pos> GetCamino()
    {
        return _camino;
    }

    LogicaTile[,] _world;
    Pos _posIni;
    Pos _posFin;

    Stack<Pos> _camino;

    //Máximo número de tiles por los que se puede mover el barco. Si es mayor que este número esta bloqueado
    const int maxWalkableTileNum = 1;


    public AEstrella(LogicaTile[,] world, Pos inicio, Pos fin)
    {
        _world = world;
        _posIni = inicio;
        _posFin = fin;

        _camino = CalculatePath();
    }

    //Distancia de un punto a otro. Solo direcciones cardinales
    int ManhattanDistance(Pos inicio,Pos fin)
    {
        return (Math.Abs(inicio.GetX() - fin.GetX()) + Math.Abs(inicio.GetY() - fin.GetY()));
    }

	//Devuelve los nodos adyacentes a los que se puede avanzar
	Queue <Pos> Neighbours(Pos pos)
    {
		int N = pos.GetY() - 1;
		int S = pos.GetY() + 1;
		int E = pos.GetX() + 1;
		int W = pos.GetX() - 1;

        Queue<Pos> adyacentes = new Queue<Pos>();

		if (N >= 0 && CanWalkHere(pos.GetX(), N))
			adyacentes.Enqueue(new Pos(pos.GetX(), N));
		if (E < GameManager.Ancho && CanWalkHere(E, pos.GetY()))
			adyacentes.Enqueue(new Pos(E, pos.GetY()));
		if (S < GameManager.Alto && CanWalkHere(pos.GetX(), S))
			adyacentes.Enqueue(new Pos(pos.GetX(), S));
		if (W >= 0 && CanWalkHere(W,pos.GetY()))
			adyacentes.Enqueue(new Pos(W, pos.GetY()));

        return adyacentes;
    }

    //Comprueba si el barco se puede mover a una posición determinada
    bool CanWalkHere(int x, int y)
    {
        return ((int)_world[y, x].GetTerreno() <= maxWalkableTileNum);
    }

    //Implementa el algoritmo A*
    Stack<Pos> CalculatePath()
    {
        Nodo nodoIni = new Nodo(null, _posIni);
        Nodo nodoFin = new Nodo(null, _posFin);

        //Calculamos el coste estimado desde este nodo hasta el destino
        nodoIni.SetF(ManhattanDistance(nodoIni.GetPos(), nodoFin.GetPos()));

        //Creamos la lista de nodos
        List <Nodo> frontera = new List<Nodo>();
        frontera.Add(nodoIni);

        //Contiene todas las casillas visitadas del tablero
        Hashtable visitados = new Hashtable();

		while (true)
		{
			//UnityEngine.Debug.Log (frontera.Count ());

            if (frontera.Count() <= 0)
                return null;

            //Encontramos el mejor nodo a expandir
            int max = GameManager.WorldSize;
            int min = -1;

            for (int i = 0; i < frontera.Count; i++)
            {
                //Encontramos el nodo de coste menor
                if (frontera[i].GetF() < max)
                {
                    max = frontera[i].GetF();
                    min = i;

                }

            }

            //Cogemos el siguiente nodo y lo quitamos de la lista
            Nodo nodoAux = frontera.ElementAt(min);
            frontera.Remove(nodoAux);

            //Comprobamos si este nodo es el destino
			if (nodoAux.GetPos() == nodoFin.GetPos())
            {
                Stack<Pos> stack = new Stack<Pos>();

                while (nodoAux != null)
                {
                    stack.Push(nodoAux.GetPos());
                    nodoAux = nodoAux.getPadre();
                }
                return stack;
            }

            //Lo añadimos a visitados
			if (!visitados.Contains((nodoAux.GetPos().ToString())))
				visitados.Add(nodoAux.GetPos().ToString(),null); //Clave,valor

			//No es el nodo resultado, hay que expandir
			Queue <Pos> adyacentes = Neighbours(nodoAux.GetPos());

			//Comprobamos todos los adyacentes alcanzables
			while (adyacentes.Count > 0)
			{
				Pos posAdy = adyacentes.Dequeue ();
				Nodo nodoAdy = new Nodo (nodoAux, posAdy );

                //Si nunca ha sido encontrado
				if (!visitados.Contains (nodoAdy.GetPos().ToString()) && !frontera.Contains (nodoAdy)) {
					//Calculamos el coste estimado desde el nodo inicio hasta este nodo
					nodoAdy.SetG (nodoAux.GetG () + ManhattanDistance (posAdy, nodoAux.GetPos ()) + (int)_world [nodoAdy.GetPos ().GetY (), nodoAdy.GetPos ().GetX ()].GetTerreno ());

					//Calculamos el coste estimado desde este nodo hasta el destino
					nodoAdy.SetF (nodoAdy.GetG () + ManhattanDistance (posAdy, nodoFin.GetPos ()));

					//Metemos este nodo en la lista 
					frontera.Add (nodoAdy);
				}

				else 
				{
					bool encontrado = false;
					int i = 0;

					while (i < frontera.Count && !encontrado)
					{
						if (frontera [i].GetPos () == nodoAdy.GetPos ()) {
							//Comprobamos si es mejor nodo el actual                      
							if (nodoAdy.GetF () < frontera [i].GetF ()) {
								//Calculamos el coste estimado desde el nodo inicio hasta este nodo
								nodoAdy.SetG (nodoAux.GetG () + ManhattanDistance (posAdy, nodoAux.GetPos ()) + (int)_world [nodoAdy.GetPos ().GetY (), nodoAdy.GetPos ().GetX ()].GetTerreno ());

								//Calculamos el coste estimado desde este nodo hasta el destino
								nodoAdy.SetF (nodoAdy.GetG () + ManhattanDistance (posAdy, nodoFin.GetPos ()));

								//Sustitumos el nodo actual por el que estaba en la lista, ya que el coste es menor

								frontera.RemoveAt (i);

								frontera.Add (nodoAdy);

							}
							encontrado = true;
						}
						i++;
					} 
				}
			}
			
		}

    }
}


