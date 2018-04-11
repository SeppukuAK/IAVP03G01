using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//Enumerado que controla los distintos estados del juego
public enum EstadoEscena { COLOCACADAVER, COLOCAAGUJERO, PAUSA, PLAY };

/// <summary>
/// Componente encargado de generar el tablero, crear y gestionar todos los objetos y su comportamiento. 
/// </summary>
public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //--------ATRIBUTOS--------------

    //--------ATRIBUTOS UNITY--------

    //Game Objects
    public GameObject tilePrefab;
    public GameObject casaPrefab;
    public GameObject cadaverPrefab;
    public GameObject detectivePrefab;
    public GameObject armaPrefab;

    //Sprites
    public Sprite spriteBarro;
    public Sprite spriteAgujero;
    public Sprite spriteSangre;
    public Sprite spriteSangreBarro;
    public Sprite spriteSangreCasa;

    //Botones
    public Button ButtonComienzaBusqueda;
    public Button ButtonPausaBusqueda;
    public Button ButtonReiniciaBusqueda;

    //Imagen y texto
    public Image IconoArma;

    public Text TextoMensaje;
    public Text TextoRelojHastaCasa;
    public Text TextoRelojHastaCadaver;
    public Text TextoPasosHastaCasa;
    public Text TextoPasosHastaCadaver;
    public Text TextoBoton;


    //--------ATRIBUTOS UNITY--------

    //CONSTANTES
    public const int ANCHO = 10;
    public const int ALTO = 5;
    public const float DISTANCIA = 0.64f;

    //VARIABLES Y GAME OBJECTS
    public Pos PosCasa { get; set; }
    public EstadoEscena Estado { get; set; }

    private Tablero tablero;
    private Agente agente;
    private bool RelojActivoHastaCasa, RelojActivoHastaCadaver;
    public bool VueltaACasa;

    private GameObject agenteGO;
    private GameObject armaGO;
    private GameObject cadaverGO;
    private GameObject[,] MatrizTiles { get; set; }//matriz de GO tiles

   
    private int pasosHastaCadaver, pasosHastaCasa;
    private int numVecesArriesgadas;

    float timer;
    float ms;
     

    //--------ATRIBUTOS UNITY--------------

    void Awake()
    {
        //GameManager es Singleton
        instance = this;
    }

    //----------INICIALIZACION--------------
    void Start()
    {
        //Inicializar GUI
        IconoArma.enabled = false;

        timer = ms = 0.0f;

        RelojActivoHastaCasa = RelojActivoHastaCadaver = VueltaACasa = false;

        numVecesArriesgadas = pasosHastaCadaver = 0;

        //Inicializamos los botones al inicio desactivados
        ButtonComienzaBusqueda.gameObject.SetActive(false);
        ButtonPausaBusqueda.gameObject.SetActive(false);
        ButtonReiniciaBusqueda.gameObject.SetActive(false);

        //Establecemos el primer estado como el de colocar el cadáver
        Estado = EstadoEscena.COLOCACADAVER;
        TextoMensaje.text = "Coloca el barco destruido";

        MatrizTiles = new GameObject[ALTO, ANCHO];
        tablero = new Tablero();

        //Generamos el tablero
        ColocaTablero();
    }

    void Update()
    {

        if (RelojActivoHastaCasa)
        {
            timer += Time.deltaTime;
            ms = (timer % 60) * 1000;

            EscribeTiempoHastaCasa();
        }

        if (RelojActivoHastaCadaver)       
            EscribeTiempoHastaCadaver();         
        
         
    }

    /// <summary>
    /// Pasa la representación lógica del tablero (matriz) a la representación física (gameobjects)
    /// </summary>
    void ColocaTablero()
    {
        GameObject GOTablero = new GameObject("Tablero");

        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                //Creamos los Game Object del tablero
                MatrizTiles[y, x] = Instantiate(tilePrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity, GOTablero.transform);

                Tile tileAux = tablero.Matriz[y, x];

                //Construimos la casilla
                MatrizTiles[y, x].GetComponent<TileView>().ConstruyeCasilla(tileAux);
            }

        }

        PosCasa = new Pos(Random.Range(0, ANCHO), Random.Range(0, ALTO));
        GameObject casa = Instantiate(casaPrefab, new Vector3(PosCasa.X * DISTANCIA, -PosCasa.Y * DISTANCIA, 0), Quaternion.identity);
    }

    //----------INICIALIZACION--------------



    //------- 1. Método al que se le llama al colocar el cadaver-------------------

    /// <summary>
    /// Método al que se le llama cuando el usuario hace click para colocar el cadáver. Se le llama desde TileView
    /// </summary>
    /// <param name="pos"></param>
    public void ColocaCadaver(Pos pos)
    {
        tablero.ColocaCadaver(pos.X, pos.Y);//Llamamos al tablero lógico para que se actualice y coloque el cadáver

        //Creamos el Game Objetct del cadáver
        cadaverGO = Instantiate(cadaverPrefab, new Vector3(pos.X * DISTANCIA, -pos.Y * DISTANCIA, 0), Quaternion.identity);

        //Cambiamos el estado al de colocar agujeros
        Estado = EstadoEscena.COLOCAAGUJERO;
        TextoMensaje.text = "Coloca los obstáculos y pulsa el botón Comienza Busqueda cuando quieras que el agente comience su busqueda";

        //Una vez colocado el cadáver, activamos el botón para que se pueda comenzar la búsqueda del agente
        ButtonComienzaBusqueda.gameObject.SetActive(true);
    }

    //------ 2. Métodos que son llamados desde Tablero.ColocaCadaver()------------

    /// <summary>
    /// Método encargado de aplicar el sprite de la sangre (tripulantes)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ColocaSpriteSangre(int x, int y)
    {
        if (HayCasa(new Pos(x, y)))
        {
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sortingOrder = 3;
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangreCasa;
        }

        else
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangre;
    }

    /// <summary>
    /// Método encargado de aplicar el sprite del arma (bote)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ColocaSpriteArma(int x, int y)
    {
        armaGO = Instantiate(armaPrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity);
    }


    //------ 2. Métodos que son llamados desde Tablero.ColocaCadaver()------------//

    //-------  1. Método al que se le llama al colocar el cadaver-------------------//


    //------- 3. Métodos que son llamados al colocar el agujero-------------------//

    /// <summary>
    /// Método al que se le llama cuando el usuario hace click para colocar el agujero. Se le llama desde TileView
    /// </summary>
    /// <param name="pos"></param>
    public void ColocaAgujero(Pos pos)
    {
        tablero.ColocaAgujero(pos.X,pos.Y); //coloca el agujero en la matriz logica

        //Aplicamos el sprite
        MatrizTiles[pos.Y, pos.X].GetComponent<SpriteRenderer>().sprite = spriteAgujero;
    }

    /// <summary>
    /// Método encargado de aplicar el sprite del barro (tripulantes flotando)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ColocaSpriteBarro(int x, int y) 
    {
        //Hacemos la distinción entre si la casilla es barro o barro con sangre
        if (tablero.Matriz[y, x].Sangre)                 
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangreBarro;         
        

        else
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteBarro;

    }

    //------- 3. Métodos que son llamados al colocar el agujero-------------------//

    /// <summary>
    /// Método que comprueba si una posición coincide con la posición de la casa(isla)
    /// </summary>
    /// <param name="pos"></param>
    public bool HayCasa(Pos pos)
    {
        return (pos.Equals(PosCasa));
    }


    //-------CALLBACKS DE BOTONES-------------------//


    /// <summary>
    /// Método que gestiona el estado PLAY en el cual el agente comienza la búsqueda
    /// </summary>
    public void IniciaBusqueda()
    {
        Estado = EstadoEscena.PLAY;
        RelojActivoHastaCasa = RelojActivoHastaCadaver = true;
        TextoMensaje.text = "";

        //Desactivamos el botón de ComienzaBúsqueda ya que ya no estamos en modo edición del mapa
        ButtonComienzaBusqueda.gameObject.SetActive(false);

        //En este caso activamos los botones de Pausar y Reiniciar 
        ButtonPausaBusqueda.gameObject.SetActive(true);
        ButtonReiniciaBusqueda.gameObject.SetActive(true);

        //Creamos al agente
        agenteGO = Instantiate(detectivePrefab, new Vector3(PosCasa.X * DISTANCIA, -PosCasa.Y * DISTANCIA, 0), Quaternion.identity);

        agente = new Agente(tablero.Matriz[PosCasa.Y, PosCasa.X]);

        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                //Oscurecemos todas las casillas
                MatrizTiles[y, x].GetComponent<SpriteRenderer>().color = Color.black;
            }

        }

        //La casilla de la casa siempre empieza iluminada (donde comienza el agente)
        MatrizTiles[PosCasa.Y, PosCasa.X].GetComponent<SpriteRenderer>().color = Color.white;

        //Desactivamos los Game Object del arma y del cadaver para que no se vean
        armaGO.GetComponent<SpriteRenderer>().enabled = false;
        cadaverGO.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine("ContinuaBusqueda");
    }


    /// <summary>
    /// Método que gestiona el estado PAUSA, en el cual se detecta cuando se pausa o se reanuda la búsqueda
    /// </summary>
    public void PausaReanudaBusqueda()
    {
        if (Estado == EstadoEscena.PLAY)
        {
            Estado = EstadoEscena.PAUSA;
            TextoBoton.text = "Reanudar";
            RelojActivoHastaCasa =  RelojActivoHastaCadaver = false;
        }
        else
        {
            Estado = EstadoEscena.PLAY;
            TextoBoton.text = "Pausa";
            RelojActivoHastaCasa = RelojActivoHastaCadaver = true;
            StartCoroutine("ContinuaBusqueda");
        }

    }

    /// <summary>
    /// Método que gestiona el estado de REINICIO, en el cual se carga de nuevo la escena
    /// </summary>
    public void ReiniciaBusqueda()
    {
        SceneManager.LoadScene("Practica3");

    }
    //-------CALLBACKS DE BOTONES-------------------//

    private bool haciendoCamino = false;//booleana que actúa como "cerrojo" avisando a la corrutina ContinuaBúsqueda cuándo ha terminado la corrutina AvanzaUnPaso

    /// <summary>
    /// Corrutina usada para que el agente vaya avanzando cada x tiempo. Espera a que la corrutina AvanzaUnPaso acabe de realizar el algoritmo para volver a ejecutarse
    /// La corrutina AvanzaUnPaso indica a este método que ha acabado poniendo haciendoCamino a false
    /// </summary>
    IEnumerator ContinuaBusqueda()
    {
        //El agente avanza en caso de no haber llegado al objetivo, haber muerto y encontrarse en el estado PLAY
        while (!agente.ObjetivoCumplido() && !agente.AgenteMuerto() && Estado == EstadoEscena.PLAY )
        {
            //Caso en el que el agente avanza un paso
            if (!haciendoCamino)
            {
                haciendoCamino = true;              
                agente.Avanza();//Avisamos al agente para que avance

                //Avanzan los pasos hacia del cadaver y hacia casa
                pasosHastaCasa++;
                pasosHastaCadaver++;
           
                EscribePasosHastaCasa();
                EscribePasosHastaCadaver();
            }
            yield return new WaitForSeconds(0.000000001f);

        }

        //Mantenemos esta corrutina en espera hasta que acabe la corrutina de AvanzaUnPaso
        while (haciendoCamino)       
           
            yield return new WaitForSeconds(0.016f);
        

        //Caso en el que el agente, sabiendo la ubicación del arma y el cadáver, vuelve a casa
        if (agente.ObjetivoCumplido())
        {
            agente.VuelveACasa();
            RelojActivoHastaCadaver = false;
           
        }
        //Texto mostrado si el agente cae por un agujero
        else if (agente.AgenteMuerto())
        {
            TextoMensaje.text = "El agente ha fallado en su misión";
            RelojActivoHastaCasa = RelojActivoHastaCadaver =  false;
        }
        
        yield return null;

    }


    /// <summary>
    /// Método que devuelve el tile según una posición dada. Necesario para el agente
    /// </summary>
    /// <param name="pos"></param>
    public Tile GetTile(Pos pos)
    {
        return tablero.Matriz[pos.Y, pos.X];
    }

    /// <summary>
    /// Método que comienza la corrutina de AvanzaUnPaso dado un camino
    /// </summary>
    /// <param name="camino"></param>
    public void MoverAgente(Stack<Pos> camino)
    {
        if (camino != null)
        {
            StartCoroutine("AvanzaUnPaso", camino);

        }

    }

    /// <summary>
    /// Corrutina que es llamada cuando la lógica del agente ha realizado las comprobaciones necesarias para poder avanzar
    /// Mueve al agente de forma física
    /// </summary>
    /// <param name="camino"></param>
    IEnumerator AvanzaUnPaso(Stack<Pos> camino)
    {
        Pos pos = new Pos(0,0);
        while (camino.Count > 0)
        {
            pos = camino.Pop();

            if (VueltaACasa)
            {
                pasosHastaCasa++;
                EscribePasosHastaCasa();

            }

            //Pasos totales

            //Movemos al agente
            agente.Pos = pos;
            agenteGO.transform.position = new Vector3(pos.X * DISTANCIA, -pos.Y * DISTANCIA, 0);      

            yield return new WaitForSeconds(0.5f);
        }

        //Comprobamos si se ha llegado a casa
        if (agente.ObjetivoCumplido() && HayCasa(pos))
            RelojActivoHastaCasa = false;

        //Comprobamos si se ha encontrado el cadáver
        if (agente.CuerpoEncontrado())
            cadaverGO.GetComponent<SpriteRenderer>().enabled = true;

        //Comprobamos si se ha encontrado el arma
        if (agente.ArmaEncontrada())
        {
            armaGO.GetComponent<SpriteRenderer>().enabled = false;
            IconoArma.enabled = true;
        }

        //Iluminamos esa casilla visitada
        MatrizTiles[pos.Y, pos.X].GetComponent<SpriteRenderer>().color = Color.white;

        //Avisamos a ContinuaBúsqueda de que esta corrutina ha terminado
       
        haciendoCamino = false;
        yield return null;
    }

    //Cambiar
    public void EscribeTiempoHastaCasa()
    {
        TextoRelojHastaCasa.text = "Tiempo hasta la casa: " + ms + "ms";
    }

    //Cambiar
    public void EscribeTiempoHastaCadaver()
    {
        TextoRelojHastaCadaver.text = "Tiempo hasta el cadáver: " + ms + "ms";
    }

    public void EscribePasosHastaCasa()
    {
        TextoPasosHastaCasa.text = "Pasos hasta la casa: " + pasosHastaCasa;
    }
    public void EscribePasosHastaCadaver()
    {
        TextoPasosHastaCadaver.text = "Pasos hasta el cadáver: " + pasosHastaCadaver;
    }

    public void AumentaNumVecesArriesgadas()
    {

    }
}
