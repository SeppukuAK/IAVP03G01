using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Clase que crea el mejor camino desde una posición inicial a una final, pasando una matriz
/// </summary>
public class AEstrella
{
    /// <summary>
    /// Nodo para el algoritmo A*
    /// </summary>
    public class Nodo
    {
        public Nodo Padre { get; set; }

        public Pos Pos { get; set; }

        /// <summary>
        /// Coste desde el inicio a este nodo
        /// </summary>
        public int F { get; set; }

        /// <summary>
        /// Coste desde este nodo al nodo objetivo
        /// </summary>
        public int G { get; set; }


        public Nodo(Nodo padre, Pos pos)
        {
            Padre = padre;
            Pos = pos;
            F = G = 0;
        }
    }

    //Atributos
    private TipoPercepcion[,] world;
    private Pos posIni;
    private Pos posFin;

    public Stack<Pos> Camino { get; set; }

    public AEstrella(TipoPercepcion[,] world, Pos inicio, Pos fin)
    {
        this.world = world;
        posIni = inicio;
        posFin = fin;

        Camino = CalculatePath();
    }

    /// <summary>
    /// Devuelve los nodos adyacentes a los que se puede avanzar
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Queue<Pos> Neighbours(Pos pos)
    {
		int N = pos.Y - 1;
		int S = pos.Y + 1;
		int E = pos.X + 1;
		int W = pos.X - 1;

        Queue<Pos> adyacentes = new Queue<Pos>();

		if (N >= 0 && CanWalkHere(pos.X, N))
			adyacentes.Enqueue(new Pos(pos.X, N));
		if (E < GameManager.ANCHO && CanWalkHere(E, pos.Y))
			adyacentes.Enqueue(new Pos(E, pos.Y));
		if (S < GameManager.ALTO && CanWalkHere(pos.X, S))
			adyacentes.Enqueue(new Pos(pos.X, S));
		if (W >= 0 && CanWalkHere(W,pos.Y))
			adyacentes.Enqueue(new Pos(W, pos.Y));

        return adyacentes;
    }

    /// <summary>
    /// Comprueba si el barco se puede mover a una posición determinada
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool CanWalkHere(int x, int y)
    {
        return (world[y, x] == TipoPercepcion.EXPLORADO);
    }

    //Implementa el algoritmo A*
    Stack<Pos> CalculatePath()
    {
        Nodo nodoIni = new Nodo(null, posIni);
        Nodo nodoFin = new Nodo(null, posFin);

        //Calculamos el coste estimado desde este nodo hasta el destino
        nodoIni.F = nodoIni.Pos.ManhattanDistance(nodoFin.Pos); 

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
			if (nodoAux.Pos.Equals(nodoFin.Pos))
            {
                Stack<Pos> stack = new Stack<Pos>();

                while (nodoAux != null)
                {
                    stack.Push(nodoAux.Pos);
                    nodoAux = nodoAux.Padre;
                }
                return stack;
            }

            //Lo añadimos a visitados
			if (!visitados.Contains((nodoAux.Pos.ToString())))
				visitados.Add(nodoAux.Pos.ToString(),null); //Clave,valor

			//No es el nodo resultado, hay que expandir
			Queue <Pos> adyacentes = Neighbours(nodoAux.Pos);

			//Comprobamos todos los adyacentes alcanzables
			while (adyacentes.Count > 0)
			{
				Pos posAdy = adyacentes.Dequeue ();
				Nodo nodoAdy = new Nodo (nodoAux, posAdy );

                //Si nunca ha sido encontrado
				if (!visitados.Contains (nodoAdy.Pos.ToString()) && !frontera.Contains (nodoAdy)) {
					//Calculamos el coste estimado desde el nodo inicio hasta este nodo
					nodoAdy.G  = nodoAux.G + posAdy.ManhattanDistance(nodoAux.Pos);

					//Calculamos el coste estimado desde este nodo hasta el destino
					nodoAdy.F =  nodoAdy.G + posAdy.ManhattanDistance(nodoFin.Pos);

                    //Metemos este nodo en la lista 
                    frontera.Add (nodoAdy);
				}

				else 
				{
					bool encontrado = false;
					int i = 0;

					while (i < frontera.Count && !encontrado)
					{
						if (frontera [i].Pos.Equals(nodoAdy.Pos)) {
							//Comprobamos si es mejor nodo el actual                      
							if (nodoAdy.F < frontera [i].F) {
                                //Calculamos el coste estimado desde el nodo inicio hasta este nodo
                                nodoAdy.G = nodoAux.G + posAdy.ManhattanDistance(nodoAux.Pos);

                                //Calculamos el coste estimado desde este nodo hasta el destino
                                nodoAdy.F = nodoAdy.G + posAdy.ManhattanDistance(nodoFin.Pos);

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


