using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Nodo
{
    private Nodo _padre;

    private Pos _pos;

    public int F { get; set; } //Coste desde el inicio a este nodo
    public int G { get; set; } //Coste desde este nodo al nodo objetivo

    public Nodo(Nodo padre, Pos pos)
    {
        _padre = padre;
        _pos = pos;
        F = G = 0;
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

}

public class AEstrella
{
    //Atributos

    //Empty si no hay camino posible
    public Stack<Pos> GetCamino()
    {
        return _camino;
    }

    TilePercepcion[,] _world;
    Pos _posIni;
    Pos _posFin;

    Stack<Pos> _camino;

    //Máximo número de tiles por los que se puede mover el barco. Si es mayor que este número esta bloqueado
    const int maxWalkableTileNum = 0;


    public AEstrella(TilePercepcion[,] world, Pos inicio, Pos fin)
    {
        _world = world;
        _posIni = inicio;
        _posFin = fin;

        _camino = CalculatePath();
    }

    //Distancia de un punto a otro. Solo direcciones cardinales
    int ManhattanDistance(Pos inicio,Pos fin)
    {
        return (Math.Abs(inicio.x - fin.x) + Math.Abs(inicio.y - fin.y));
    }

	//Devuelve los nodos adyacentes a los que se puede avanzar
	Queue <Pos> Neighbours(Pos pos)
    {
		int N = pos.y - 1;
		int S = pos.y + 1;
		int E = pos.x + 1;
		int W = pos.x - 1;

        Queue<Pos> adyacentes = new Queue<Pos>();

		if (N >= 0 && CanWalkHere(pos.x, N))
			adyacentes.Enqueue(new Pos(pos.x, N));
		if (E < GameManager.ANCHO && CanWalkHere(E, pos.y))
			adyacentes.Enqueue(new Pos(E, pos.y));
		if (S < GameManager.ALTO && CanWalkHere(pos.x, S))
			adyacentes.Enqueue(new Pos(pos.x, S));
		if (W >= 0 && CanWalkHere(W,pos.y))
			adyacentes.Enqueue(new Pos(W, pos.y));

        return adyacentes;
    }

    //Comprueba si el barco se puede mover a una posición determinada
    bool CanWalkHere(int x, int y)
    {
        return (_world[y, x].Percepcion == TipoPercepcion.EXPLORADO);
    }

    //Implementa el algoritmo A*
    Stack<Pos> CalculatePath()
    {
        Nodo nodoIni = new Nodo(null, _posIni);
        Nodo nodoFin = new Nodo(null, _posFin);

        //Calculamos el coste estimado desde este nodo hasta el destino
        nodoIni.F = ManhattanDistance(nodoIni.GetPos(), nodoFin.GetPos());

        //Creamos la lista de nodos
        List <Nodo> frontera = new List<Nodo>();
        frontera.Add(nodoIni);

        //Contiene todas las casillas visitadas del tablero
        Hashtable visitados = new Hashtable();

		while (true)
		{
            if (frontera.Count() <= 0)
                return null;

            //Encontramos el mejor nodo a expandir
            Nodo nodoAux = frontera
            .OrderBy(t => t.F)
            .FirstOrDefault();

            //Cogemos el siguiente nodo y lo quitamos de la lista
            frontera.Remove(nodoAux);

            //Comprobamos si este nodo es el destino
			if (nodoAux.GetPos().Equals(nodoFin.GetPos()))
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
					nodoAdy.G  = nodoAux.G + ManhattanDistance (posAdy, nodoAux.GetPos ());

					//Calculamos el coste estimado desde este nodo hasta el destino
					nodoAdy.F =  nodoAdy.G + ManhattanDistance (posAdy, nodoFin.GetPos ());

					//Metemos este nodo en la lista 
					frontera.Add (nodoAdy);
				}

				else 
				{
					bool encontrado = false;
					int i = 0;

					while (i < frontera.Count && !encontrado)
					{
						if (frontera [i].GetPos ().Equals(nodoAdy.GetPos ())) {
							//Comprobamos si es mejor nodo el actual                      
							if (nodoAdy.F < frontera [i].F) {
								//Calculamos el coste estimado desde el nodo inicio hasta este nodo
								nodoAdy.G =  nodoAux.G + ManhattanDistance (posAdy, nodoAux.GetPos ()) + 1;

								//Calculamos el coste estimado desde este nodo hasta el destino
								nodoAdy.F =  nodoAdy.G  + ManhattanDistance (posAdy, nodoFin.GetPos ());

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


