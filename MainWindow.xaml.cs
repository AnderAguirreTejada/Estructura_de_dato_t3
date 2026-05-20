using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TowerDefenseWPF.GameLogic;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF;

/// <summary>
/// Lógica principal del juego (code-behind).
///
/// Estructuras de datos usadas (resumen):
///   - <c>List&lt;Torre&gt; _torres</c>, <c>List&lt;Enemigo&gt; _enemigos</c>,
///     <c>List&lt;Proyectil&gt; _proyectiles</c>: colecciones dinámicas que
///     el game loop recorre cada frame.
///   - <c>Queue&lt;GeneracionEnemigo&gt;</c> en <see cref="GestorOleadas"/>: orden FIFO
///     de aparición de enemigos.
///   - <c>Stack&lt;AccionJugador&gt;</c> en <see cref="HistorialAcciones"/>: pila LIFO
///     para deshacer la última acción del jugador.
///   - Árbol n-ario de <see cref="NodoMejora"/> en cada torre: ramas de mejora.
/// </summary>
public partial class MainWindow : Window, IContextoJuego
{
    private const int OroInicial = 220;
    private const int VidasIniciales = 20;

    private readonly List<Torre> _torres = new();
    private readonly List<Enemigo> _enemigos = new();
    private readonly List<Proyectil> _proyectiles = new();

    private GestorOleadas _gestorOleadas;
    private readonly HistorialAcciones _historial = new();

    private int _oro = OroInicial;
    private int _vidas = VidasIniciales;

    private DispatcherTimer _temporizador = null!;
    private DateTime _ultimoTick;
    private bool _juegoTerminado;

    private TipoTorre? _tipoColocando;
    private Rectangle? _torreFantasma;
    private Ellipse? _rangoFantasma;

    private Torre? _torreSelecionada;
    private Ellipse? _indicadorRangoSeleccionado;

    private readonly Dictionary<TipoTorre, Button> _botonesDeTorre = new();

    public MainWindow()
    {
        InitializeComponent();
        _gestorOleadas = new GestorOleadas(BibliotecaOleadas.ConstruirTodasLasOleadas());
        Loaded += AlCargar;
    }

    private void AlCargar(object sender, RoutedEventArgs e)
    {
        DibujarCamino();
        ConstruirBotonesDeTorre();
        ActualizarEstadísticas();
        StatusLabel.Text = "Pulsa \"Iniciar oleada\" cuando estés listo.";

        _temporizador = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _temporizador.Tick += TickDelJuego;
        _ultimoTick = DateTime.Now;
        _temporizador.Start();
    }

    // ===================== BUCLE DE JUEGO =====================

    private void TickDelJuego(object? sender, EventArgs e)
    {
        if (_juegoTerminado) return;

        var now = DateTime.Now;
        double dt = (now - _ultimoTick).TotalSeconds;
        _ultimoTick = now;
        if (dt > 0.1) dt = 0.1;

        var proximaGeneracion = _gestorOleadas.Tick(dt);
        if (proximaGeneracion.HasValue) GenerarEnemigo(proximaGeneracion.Value);

        for (int i = _enemigos.Count - 1; i >= 0; i--)
        {
            var enemigo = _enemigos[i];
            ActualizarEnemigo(enemigo, dt);
            if (enemigo.LlegoAlFinal)
            {
                EliminarEnemigo(enemigo);
                _vidas--;
                if (_vidas <= 0) { JuegoPerdido(); return; }
            }
        }

        foreach (var torre in _torres)
        {
            torre.EnfriamientoDisparo -= dt;
            if (torre.EnfriamientoDisparo > 0) continue;
            var objetivo = BuscarObjetivo(torre);
            if (objetivo == null) continue;
            DispararProyectil(torre, objetivo);
            torre.EnfriamientoDisparo = 1.0 / torre.VelocidadDisparo;
        }

        for (int i = _proyectiles.Count - 1; i >= 0; i--)
        {
            var p = _proyectiles[i];
            ActualizarProyectil(p, dt);
            if (p.EstaTerminado) EliminarProyectil(p);
        }

        ActualizarEstadísticas();
        VerificarCondiciónDeVictoria();
    }

    // ===================== ENEMIGOS =====================

    private void GenerarEnemigo(TipoEnemigo tipo)
    {
        var enemigo = FabricaEnemigo.Crear(tipo);
        enemigo.Posicion = DatosMapa.PuntosControl[0];
        enemigo.SiguientePuntoControlIndex = 1;

        var color = tipo switch
        {
            TipoEnemigo.Normal => Color.FromRgb(0xE7, 0x4C, 0x3C),
            TipoEnemigo.Rapido => Color.FromRgb(0xF1, 0xC4, 0x0F),
            TipoEnemigo.Tanque => Color.FromRgb(0x7F, 0x8C, 0x8D),
            _ => Colors.White
        };

        enemigo.Cuerpo = new Ellipse
        {
            Width = enemigo.Radio * 2,
            Height = enemigo.Radio * 2,
            Fill = new SolidColorBrush(color),
            Stroke = Brushes.Black,
            StrokeThickness = 1.5
        };
        enemigo.FondoBarraVida = new Rectangle
        {
            Width = enemigo.Radio * 2.5,
            Height = 4,
            Fill = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22))
        };
        enemigo.BarraVida = new Rectangle
        {
            Width = enemigo.Radio * 2.5,
            Height = 4,
            Fill = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71))
        };

        Panel.SetZIndex(enemigo.Cuerpo, 50);
        Panel.SetZIndex(enemigo.FondoBarraVida, 51);
        Panel.SetZIndex(enemigo.BarraVida, 52);

        GameCanvas.Children.Add(enemigo.Cuerpo);
        GameCanvas.Children.Add(enemigo.FondoBarraVida);
        GameCanvas.Children.Add(enemigo.BarraVida);

        _enemigos.Add(enemigo);
        ActualizarPosicionVisualEnemigo(enemigo);
    }

    private void ActualizarEnemigo(Enemigo enemigo, double dt)
    {
        if (enemigo.TiempoRalentiRestante > 0)
        {
            enemigo.TiempoRalentiRestante -= dt;
            if (enemigo.TiempoRalentiRestante <= 0)
            {
                enemigo.FactorRalenti = 1.0;
                enemigo.TiempoRalentiRestante = 0;
            }
        }

        if (enemigo.SiguientePuntoControlIndex >= DatosMapa.PuntosControl.Count)
        {
            enemigo.LlegoAlFinal = true;
            return;
        }

        var objetivo = DatosMapa.PuntosControl[enemigo.SiguientePuntoControlIndex];
        var dx = objetivo.X - enemigo.Posicion.X;
        var dy = objetivo.Y - enemigo.Posicion.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);
        var paso = enemigo.VelocidadActual * dt;

        if (paso >= dist)
        {
            enemigo.Posicion = objetivo;
            enemigo.SiguientePuntoControlIndex++;
        }
        else
        {
            enemigo.Posicion = new Point(
                enemigo.Posicion.X + dx / dist * paso,
                enemigo.Posicion.Y + dy / dist * paso);
        }

        ActualizarPosicionVisualEnemigo(enemigo);
    }

    private void ActualizarPosicionVisualEnemigo(Enemigo enemigo)
    {
        Canvas.SetLeft(enemigo.Cuerpo, enemigo.Posicion.X - enemigo.Radio);
        Canvas.SetTop(enemigo.Cuerpo, enemigo.Posicion.Y - enemigo.Radio);

        double anchoBarraVida = enemigo.Radio * 2.5;
        Canvas.SetLeft(enemigo.FondoBarraVida, enemigo.Posicion.X - anchoBarraVida / 2);
        Canvas.SetTop(enemigo.FondoBarraVida, enemigo.Posicion.Y - enemigo.Radio - 8);
        enemigo.BarraVida.Width = anchoBarraVida * Math.Max(0, enemigo.Vida / enemigo.VidaMaxima);
        Canvas.SetLeft(enemigo.BarraVida, enemigo.Posicion.X - anchoBarraVida / 2);
        Canvas.SetTop(enemigo.BarraVida, enemigo.Posicion.Y - enemigo.Radio - 8);
    }

    private void EliminarEnemigo(Enemigo enemigo)
    {
        GameCanvas.Children.Remove(enemigo.Cuerpo);
        GameCanvas.Children.Remove(enemigo.FondoBarraVida);
        GameCanvas.Children.Remove(enemigo.BarraVida);
        _enemigos.Remove(enemigo);
    }

    private void DañarEnemigo(Enemigo enemigo, double daño)
    {
        enemigo.Vida -= daño;
        if (enemigo.Vida <= 0)
        {
            _oro += enemigo.Recompensa;
            EliminarEnemigo(enemigo);
        }
        else
        {
            ActualizarPosicionVisualEnemigo(enemigo);
        }
    }

    // ===================== TORRES =====================

    private Enemigo? BuscarObjetivo(Torre torre)
    {
        Enemigo? mejor = null;
        int mejorPuntoControl = -1;
        double mejorProgresoAtrasado = -1;

        foreach (var enemigo in _enemigos)
        {
            if (enemigo.EstaMuerto || enemigo.LlegoAlFinal) continue;
            double dx = enemigo.Posicion.X - torre.Posicion.X;
            double dy = enemigo.Posicion.Y - torre.Posicion.Y;
            if (dx * dx + dy * dy > torre.Rango * torre.Rango) continue;

            double progreso = enemigo.SiguientePuntoControlIndex * 1000;
            if (enemigo.SiguientePuntoControlIndex > 0 && enemigo.SiguientePuntoControlIndex <= DatosMapa.PuntosControl.Count)
            {
                var pc = DatosMapa.PuntosControl[enemigo.SiguientePuntoControlIndex - 1];
                double pdx = enemigo.Posicion.X - pc.X;
                double pdy = enemigo.Posicion.Y - pc.Y;
                progreso += Math.Sqrt(pdx * pdx + pdy * pdy);
            }
            if (progreso > mejorProgresoAtrasado)
            {
                mejorProgresoAtrasado = progreso;
                mejorPuntoControl = enemigo.SiguientePuntoControlIndex;
                mejor = enemigo;
            }
        }
        return mejor;
    }

    private void DispararProyectil(Torre torre, Enemigo objetivo)
    {
        var colorProyectil = torre.Tipo switch
        {
            TipoTorre.Arquero => Color.FromRgb(0xFF, 0xF1, 0xC0),
            TipoTorre.Cañon => Color.FromRgb(0xFF, 0x88, 0x33),
            TipoTorre.Mago => Color.FromRgb(0x66, 0xCC, 0xFF),
            _ => Colors.White
        };
        double radio = torre.Tipo == TipoTorre.Cañon ? 6 : 4;

        var cuerpo = new Ellipse
        {
            Width = radio * 2,
            Height = radio * 2,
            Fill = new SolidColorBrush(colorProyectil),
            Stroke = Brushes.Black,
            StrokeThickness = 0.8
        };
        Panel.SetZIndex(cuerpo, 60);
        GameCanvas.Children.Add(cuerpo);

        var p = new Proyectil
        {
            Posicion = torre.Posicion,
            PosicionObjetivo = objetivo.Posicion,
            Objetivo = objetivo,
            Velocidad = torre.VelocidadProyectil,
            Daño = torre.Daño,
            EsExplosivo = torre.EsExplosivo,
            RadioExplosion = torre.RadioExplosion,
            CantidadRalenti = torre.CantidadRalenti,
            DuracionRalenti = torre.DuracionRalenti,
            Cuerpo = cuerpo
        };
        Canvas.SetLeft(cuerpo, p.Posicion.X - radio);
        Canvas.SetTop(cuerpo, p.Posicion.Y - radio);
        _proyectiles.Add(p);
    }

    private void ActualizarProyectil(Proyectil p, double dt)
    {
        if (!p.Objetivo.EstaMuerto && !p.Objetivo.LlegoAlFinal)
            p.PosicionObjetivo = p.Objetivo.Posicion;

        double dx = p.PosicionObjetivo.X - p.Posicion.X;
        double dy = p.PosicionObjetivo.Y - p.Posicion.Y;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        double paso = p.Velocidad * dt;

        if (paso >= dist || dist < 4)
        {
            p.Posicion = p.PosicionObjetivo;
            AplicarImpacto(p);
            p.EstaTerminado = true;
            return;
        }

        p.Posicion = new Point(
            p.Posicion.X + dx / dist * paso,
            p.Posicion.Y + dy / dist * paso);

        double r = ((Ellipse)p.Cuerpo).Width / 2;
        Canvas.SetLeft(p.Cuerpo, p.Posicion.X - r);
        Canvas.SetTop(p.Cuerpo, p.Posicion.Y - r);
    }

    private void AplicarImpacto(Proyectil p)
    {
        if (p.EsExplosivo)
        {
            var impactados = new List<Enemigo>();
            foreach (var enemigo in _enemigos)
            {
                if (enemigo.EstaMuerto || enemigo.LlegoAlFinal) continue;
                double dx = enemigo.Posicion.X - p.Posicion.X;
                double dy = enemigo.Posicion.Y - p.Posicion.Y;
                if (dx * dx + dy * dy <= p.RadioExplosion * p.RadioExplosion)
                    impactados.Add(enemigo);
            }
            foreach (var e in impactados) DañarEnemigo(e, p.Daño);
            MostrarAnilloDeImpacto(p.Posicion, p.RadioExplosion);
        }
        else
        {
            if (!p.Objetivo.EstaMuerto && !p.Objetivo.LlegoAlFinal)
            {
                DañarEnemigo(p.Objetivo, p.Daño);
                if (p.CantidadRalenti > 0 && !p.Objetivo.EstaMuerto)
                {
                    double nuevoFactor = 1.0 - p.CantidadRalenti;
                    if (nuevoFactor < p.Objetivo.FactorRalenti)
                        p.Objetivo.FactorRalenti = nuevoFactor;
                    p.Objetivo.TiempoRalentiRestante = Math.Max(p.Objetivo.TiempoRalentiRestante, p.DuracionRalenti);
                }
            }
        }
    }

    private void EliminarProyectil(Proyectil p)
    {
        GameCanvas.Children.Remove(p.Cuerpo);
        _proyectiles.Remove(p);
    }

    private void MostrarAnilloDeImpacto(Point pos, double radio)
    {
        var anillo = new Ellipse
        {
            Width = radio * 2,
            Height = radio * 2,
            Stroke = new SolidColorBrush(Color.FromArgb(220, 0xFF, 0xAA, 0x33)),
            StrokeThickness = 3,
            Fill = new SolidColorBrush(Color.FromArgb(80, 0xFF, 0x88, 0x22)),
            IsHitTestVisible = false
        };
        Canvas.SetLeft(anillo, pos.X - radio);
        Canvas.SetTop(anillo, pos.Y - radio);
        Panel.SetZIndex(anillo, 55);
        GameCanvas.Children.Add(anillo);

        var anim = new DoubleAnimation
        {
            From = 1, To = 0,
            Duration = TimeSpan.FromSeconds(0.3)
        };
        anim.Completed += (_, _) => GameCanvas.Children.Remove(anillo);
        anillo.BeginAnimation(OpacityProperty, anim);
    }

    // ===================== ENTRADA / CONTROLES =====================

    private void AlHacerClicEnLienzo_Presionado(object sender, MouseButtonEventArgs e)
    {
        if (_juegoTerminado) return;

        if (e.ChangedButton == MouseButton.Right)
        {
            if (_tipoColocando != null) CancelarColocacion();
            else DeseleccionarTorre();
            return;
        }

        var pos = e.GetPosition(GameCanvas);

        if (_tipoColocando != null)
        {
            if (PuedeColocarseen(pos)) ColocarTorre(_tipoColocando.Value, pos);
            else StatusLabel.Text = "No puedes colocar ahí.";
            return;
        }

        var torre = BuscarTorresCercanas(pos, 22);
        if (torre != null) SeleccionarTorre(torre);
        else DeseleccionarTorre();
    }

    private void AlMoverEnLienzo(object sender, MouseEventArgs e)
    {
        if (_tipoColocando == null) return;
        ActualizarPosicionFantasma(e.GetPosition(GameCanvas));
    }

    private void AlHacerClicEnBotondeTorre(TipoTorre tipo)
    {
        if (_juegoTerminado) return;
        if (_tipoColocando == tipo) { CancelarColocacion(); return; }
        _tipoColocando = tipo;
        DeseleccionarTorre();
        ConstruirFantasma();
        ActualizarResaltadoBotonesTorre();
        StatusLabel.Text = $"Coloca {tipo} (click). Click derecho para cancelar.";
    }

    // ===================== COLOCACIÓN DE TORRES =====================

    private void ConstruirFantasma()
    {
        if (_tipoColocando == null) return;
        if (_torreFantasma != null) GameCanvas.Children.Remove(_torreFantasma);
        if (_rangoFantasma != null) GameCanvas.Children.Remove(_rangoFantasma);

        var muestra = FabricaTorre.Crear(_tipoColocando.Value, new Point(0, 0));
        _rangoFantasma = new Ellipse
        {
            Width = muestra.Rango * 2,
            Height = muestra.Rango * 2,
            Stroke = new SolidColorBrush(Color.FromArgb(180, 0xFF, 0xFF, 0xFF)),
            StrokeThickness = 1.5,
            Fill = new SolidColorBrush(Color.FromArgb(40, 0xFF, 0xFF, 0xFF)),
            IsHitTestVisible = false
        };
        _torreFantasma = new Rectangle
        {
            Width = 28,
            Height = 28,
            Fill = new SolidColorBrush(ColorParaTorre(_tipoColocando.Value)) { Opacity = 0.7 },
            Stroke = Brushes.White,
            StrokeThickness = 1,
            IsHitTestVisible = false,
            Opacity = 0.8
        };
        Panel.SetZIndex(_rangoFantasma, 200);
        Panel.SetZIndex(_torreFantasma, 201);
        GameCanvas.Children.Add(_rangoFantasma);
        GameCanvas.Children.Add(_torreFantasma);
    }

    private void ActualizarPosicionFantasma(Point p)
    {
        if (_torreFantasma == null || _rangoFantasma == null) return;
        Canvas.SetLeft(_torreFantasma, p.X - 14);
        Canvas.SetTop(_torreFantasma, p.Y - 14);
        Canvas.SetLeft(_rangoFantasma, p.X - _rangoFantasma.Width / 2);
        Canvas.SetTop(_rangoFantasma, p.Y - _rangoFantasma.Height / 2);

        bool valido = PuedeColocarseen(p);
        _rangoFantasma.Fill = new SolidColorBrush(valido
            ? Color.FromArgb(50, 0x33, 0xCC, 0x55)
            : Color.FromArgb(50, 0xCC, 0x33, 0x33));
    }

    private bool PuedeColocarseen(Point p)
    {
        if (_tipoColocando == null) return false;
        if (_oro < FabricaTorre.CostoDe(_tipoColocando.Value)) return false;
        if (p.X < 16 || p.X > DatosMapa.AnchuraLienzo - 16) return false;
        if (p.Y < 16 || p.Y > DatosMapa.AltoLienzo - 16) return false;
        if (DatosMapa.EstáEnCamino(p, 22)) return false;
        foreach (var t in _torres)
        {
            double dx = t.Posicion.X - p.X;
            double dy = t.Posicion.Y - p.Y;
            if (dx * dx + dy * dy < 34 * 34) return false;
        }
        return true;
    }

    private void ColocarTorre(TipoTorre tipo, Point pos)
    {
        int costo = FabricaTorre.CostoDe(tipo);
        if (_oro < costo) return;
        _oro -= costo;

        var torre = FabricaTorre.Crear(tipo, pos);
        var forma = new Rectangle
        {
            Width = 28,
            Height = 28,
            Fill = new SolidColorBrush(ColorParaTorre(tipo)),
            Stroke = Brushes.Black,
            StrokeThickness = 1.5,
            RadiusX = 4,
            RadiusY = 4
        };
        Canvas.SetLeft(forma, pos.X - 14);
        Canvas.SetTop(forma, pos.Y - 14);
        Panel.SetZIndex(forma, 30);
        GameCanvas.Children.Add(forma);
        torre.Cuerpo = forma;

        _torres.Add(torre);
        _historial.Agregar(new AccionColocarTorre(torre, costo));

        CancelarColocacion();
        ActualizarEstadísticas();
    }

    private void CancelarColocacion()
    {
        _tipoColocando = null;
        if (_torreFantasma != null) { GameCanvas.Children.Remove(_torreFantasma); _torreFantasma = null; }
        if (_rangoFantasma != null) { GameCanvas.Children.Remove(_rangoFantasma); _rangoFantasma = null; }
        ActualizarResaltadoBotonesTorre();
        if (StatusLabel.Text.StartsWith("Coloca")) StatusLabel.Text = "";
    }

    private static Color ColorParaTorre(TipoTorre tipo) => tipo switch
    {
        TipoTorre.Arquero => Color.FromRgb(0xC8, 0x8E, 0x3D),
        TipoTorre.Cañon => Color.FromRgb(0x55, 0x5A, 0x6B),
        TipoTorre.Mago => Color.FromRgb(0x9B, 0x6E, 0xD8),
        _ => Colors.White
    };

    // ===================== SELECCIÓN Y MEJORAS DE TORRES =====================

    private Torre? BuscarTorresCercanas(Point p, double radio)
    {
        foreach (var t in _torres)
        {
            double dx = t.Posicion.X - p.X;
            double dy = t.Posicion.Y - p.Y;
            if (dx * dx + dy * dy <= radio * radio) return t;
        }
        return null;
    }

    private void SeleccionarTorre(Torre torre)
    {
        _torreSelecionada = torre;
        if (_indicadorRangoSeleccionado == null)
        {
            _indicadorRangoSeleccionado = new Ellipse
            {
                Stroke = new SolidColorBrush(Color.FromArgb(220, 0xFF, 0xCC, 0x44)),
                StrokeThickness = 1.5,
                Fill = new SolidColorBrush(Color.FromArgb(35, 0xFF, 0xCC, 0x44)),
                IsHitTestVisible = false
            };
            Panel.SetZIndex(_indicadorRangoSeleccionado, 25);
            GameCanvas.Children.Add(_indicadorRangoSeleccionado);
        }
        ActualizarIndicadorRango(torre);
        _indicadorRangoSeleccionado.Visibility = Visibility.Visible;
        ActualizarPanelMejoras();
    }

    private void ActualizarIndicadorRango(Torre torre)
    {
        if (_indicadorRangoSeleccionado == null) return;
        _indicadorRangoSeleccionado.Width = torre.Rango * 2;
        _indicadorRangoSeleccionado.Height = torre.Rango * 2;
        Canvas.SetLeft(_indicadorRangoSeleccionado, torre.Posicion.X - torre.Rango);
        Canvas.SetTop(_indicadorRangoSeleccionado, torre.Posicion.Y - torre.Rango);
    }

    private void DeseleccionarTorre()
    {
        _torreSelecionada = null;
        if (_indicadorRangoSeleccionado != null)
            _indicadorRangoSeleccionado.Visibility = Visibility.Collapsed;
        UpgradePanel.Visibility = Visibility.Collapsed;
    }

    private void ActualizarPanelMejoras()
    {
        if (_torreSelecionada == null)
        {
            UpgradePanel.Visibility = Visibility.Collapsed;
            return;
        }
        var t = _torreSelecionada;
        UpgradePanel.Visibility = Visibility.Visible;
        SelectedTowerLabel.Text = $"{NombreDeTorre(t.Tipo)} — {t.NodoActual.Nombre}";

        var estadisticas = $"Daño: {t.Daño:0.#}   Ritmo: {t.VelocidadDisparo:0.##}/s\n" +
                    $"Rango: {t.Rango:0}";
        if (t.EsExplosivo) estadisticas += $"   Splash: {t.RadioExplosion:0}";
        if (t.CantidadRalenti > 0) estadisticas += $"\nRalentiza: {(int)(t.CantidadRalenti * 100)}%  ({t.DuracionRalenti:0.#}s)";
        estadisticas += $"\nInvertido: {t.OroTotalInvertido} oro";
        TowerStatsLabel.Text = estadisticas;

        DibujarArbolMejoras(t);
        ConstruirBotonesMejoras(t);
    }

    private static string NombreDeTorre(TipoTorre t) => t switch
    {
        TipoTorre.Arquero => "Arquero",
        TipoTorre.Cañon => "Cañón",
        TipoTorre.Mago => "Mago",
        _ => t.ToString()
    };

    private void DibujarArbolMejoras(Torre torre)
    {
        TreeCanvas.Children.Clear();
        var posiciones = new Dictionary<NodoMejora, Point>();
        DisponerArbol(torre.RaizMejora, new Point(120, 18), 100, 42, posiciones);

        foreach (var (nodo, pos) in posiciones)
        {
            foreach (var hijo in nodo.Hijos)
            {
                if (!posiciones.TryGetValue(hijo, out var posFijo)) continue;
                bool enCamino = EstaEnCaminoActual(nodo, hijo, torre.NodoActual);
                var linea = new Line
                {
                    X1 = pos.X, Y1 = pos.Y, X2 = posFijo.X, Y2 = posFijo.Y,
                    Stroke = new SolidColorBrush(enCamino
                        ? Color.FromRgb(0xFF, 0xCC, 0x44)
                        : Color.FromRgb(0x44, 0x44, 0x5A)),
                    StrokeThickness = enCamino ? 2 : 1
                };
                TreeCanvas.Children.Add(linea);
            }
        }

        foreach (var (nodo, pos) in posiciones)
        {
            bool esActual = nodo == torre.NodoActual;
            bool estaDisponible = torre.NodoActual.Hijos.Contains(nodo);
            bool estaEnCamino = esActual || EsAncesor(nodo, torre.NodoActual);

            Color relleno = esActual ? Color.FromRgb(0xFF, 0xCC, 0x44)
                       : estaEnCamino ? Color.FromRgb(0xFF, 0xAA, 0x55)
                       : estaDisponible ? Color.FromRgb(0x88, 0x88, 0xB0)
                       : Color.FromRgb(0x44, 0x44, 0x55);
            double r = esActual ? 8 : 6;
            var punto = new Ellipse
            {
                Width = r * 2,
                Height = r * 2,
                Fill = new SolidColorBrush(relleno),
                Stroke = Brushes.Black,
                StrokeThickness = 0.7
            };
            Canvas.SetLeft(punto, pos.X - r);
            Canvas.SetTop(punto, pos.Y - r);
            TreeCanvas.Children.Add(punto);
        }
    }

    private static void DisponerArbol(NodoMejora nodo, Point pos, double espacioHorizontal, double espacioVertical,
                                   Dictionary<NodoMejora, Point> resultado)
    {
        resultado[nodo] = pos;
        int n = nodo.Hijos.Count;
        if (n == 0) return;
        double inicioX = pos.X - espacioHorizontal * (n - 1) / 2.0;
        for (int i = 0; i < n; i++)
        {
            var posFijo = new Point(inicioX + i * espacioHorizontal, pos.Y + espacioVertical);
            DisponerArbol(nodo.Hijos[i], posFijo, espacioHorizontal / 1.9, espacioVertical, resultado);
        }
    }

    private static bool EsAncesor(NodoMejora candidato, NodoMejora descendiente)
    {
        var n = descendiente.Padre;
        while (n != null)
        {
            if (n == candidato) return true;
            n = n.Padre;
        }
        return false;
    }

    private static bool EstaEnCaminoActual(NodoMejora desde, NodoMejora hacia, NodoMejora actual)
    {
        if (desde == actual && actual.Hijos.Contains(hacia)) return false;
        return (hacia == actual || EsAncesor(hacia, actual)) && (desde == actual.Padre || EsAncesor(desde, actual));
    }

    private void ConstruirBotonesMejoras(Torre torre)
    {
        UpgradeOptionsPanel.Children.Clear();
        if (torre.NodoActual.Hijos.Count == 0)
        {
            UpgradeOptionsPanel.Children.Add(new TextBlock
            {
                Text = "Sin más mejoras disponibles.",
                Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99)),
                FontStyle = FontStyles.Italic,
                FontSize = 11
            });
            return;
        }

        foreach (var hijo in torre.NodoActual.Hijos)
        {
            var btn = new Button
            {
                Margin = new Thickness(0, 3, 0, 0),
                IsEnabled = _oro >= hijo.Costo,
                Tag = hijo
            };
            var pila = new StackPanel();
            pila.Children.Add(new TextBlock
            {
                Text = $"{hijo.Nombre}  ({hijo.Costo} oro)",
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 12
            });
            pila.Children.Add(new TextBlock
            {
                Text = hijo.Descripcion,
                Foreground = new SolidColorBrush(Color.FromRgb(0xBB, 0xBB, 0xCC)),
                FontSize = 11
            });
            btn.Content = pila;
            btn.Click += (_, _) => ComprarMejora(torre, hijo);
            UpgradeOptionsPanel.Children.Add(btn);
        }
    }

    private void ComprarMejora(Torre torre, NodoMejora hijo)
    {
        if (_oro < hijo.Costo) return;
        _oro -= hijo.Costo;

        var anterior = torre.NodoActual;
        hijo.Aplicar(torre);
        torre.NodoActual = hijo;
        torre.OroTotalInvertido += hijo.Costo;

        _historial.Agregar(new AccionMejorarTorre(torre, anterior, hijo, hijo.Costo));

        if (_torreSelecionada == torre)
        {
            ActualizarIndicadorRango(torre);
            ActualizarPanelMejoras();
        }
        ActualizarEstadísticas();
    }

    private void BotónVenderTorre_Click(object sender, RoutedEventArgs e)
    {
        if (_torreSelecionada == null || _juegoTerminado) return;
        var t = _torreSelecionada;
        int reembolso = (int)(t.OroTotalInvertido * 0.6);
        _oro += reembolso;
        EliminarTorreSilenciosamente(t);
        DeseleccionarTorre();
        _historial.Limpiar();
        StatusLabel.Text = $"Vendida: +{reembolso} oro. (Historial de Deshacer limpiado.)";
        ActualizarEstadísticas();
    }

    // ===================== DESHACER (PILA LIFO) =====================

    private void BotónDeshacer_Click(object sender, RoutedEventArgs e)
    {
        if (_juegoTerminado) return;
        var accion = _historial.Sacar();
        if (accion == null) return;
        accion.Deshacer(this);
        StatusLabel.Text = $"Deshecho: {accion.Descripcion}";

        if (_torreSelecionada != null && _torres.Contains(_torreSelecionada))
        {
            ActualizarIndicadorRango(_torreSelecionada);
            ActualizarPanelMejoras();
        }
        else if (_torreSelecionada != null)
        {
            DeseleccionarTorre();
        }
        ActualizarEstadísticas();
    }

    // ===================== CONTEXTO DE JUEGO (PARA DESHACER) =====================

    public void EliminarTorreSilenciosamente(Torre torre)
    {
        GameCanvas.Children.Remove(torre.Cuerpo);
        _torres.Remove(torre);
        if (_torreSelecionada == torre) DeseleccionarTorre();
    }

    public void AñadirOro(int cantidad) => _oro += cantidad;

    // ===================== OLEADAS =====================

    private void BotónIniciarOleada_Click(object sender, RoutedEventArgs e)
    {
        if (_juegoTerminado) return;
        if (_gestorOleadas.IntentarIniciarSiguienteOleada())
        {
            StatusLabel.Text = $"Oleada {_gestorOleadas.NúmeroOleadaActual} iniciada.";
            ActualizarEstadísticas();
        }
    }

    // ===================== INTERFAZ / ESTADÍSTICAS =====================

    private void ConstruirBotonesDeTorre()
    {
        foreach (TipoTorre tipo in Enum.GetValues<TipoTorre>())
        {
            var btn = new Button { Margin = new Thickness(0, 3, 0, 3), Tag = tipo };
            var pila = new StackPanel();
            pila.Children.Add(new TextBlock
            {
                Text = NombreDeTorre(tipo),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 13
            });
            pila.Children.Add(new TextBlock
            {
                Text = $"Costo: {FabricaTorre.CostoDe(tipo)} oro",
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00)),
                FontSize = 11
            });
            btn.Content = pila;
            btn.Click += (_, _) => AlHacerClicEnBotondeTorre(tipo);
            TowerButtonsPanel.Children.Add(btn);
            _botonesDeTorre[tipo] = btn;
        }
    }

    private void ActualizarResaltadoBotonesTorre()
    {
        foreach (var (tipo, btn) in _botonesDeTorre)
        {
            btn.BorderBrush = tipo == _tipoColocando
                ? new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x44))
                : new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x6F));
            btn.BorderThickness = tipo == _tipoColocando ? new Thickness(2) : new Thickness(1);
        }
    }

    private void ActualizarEstadísticas()
    {
        GoldLabel.Text = _oro.ToString();
        LivesLabel.Text = _vidas.ToString();
        WaveLabel.Text = $"{Math.Max(0, _gestorOleadas.NúmeroOleadaActual)} / {_gestorOleadas.TotalOleadas}";
        QueueLabel.Text = _gestorOleadas.ColaActual.Count.ToString();

        StartWaveButton.IsEnabled = !_gestorOleadas.EsOleadaActiva && _gestorOleadas.HayMásOleadas && !_juegoTerminado;
        StartWaveButton.Content = _gestorOleadas.ÍndiceOleadaActual < 0
            ? "▶ Iniciar primera oleada"
            : $"▶ Iniciar oleada {_gestorOleadas.NúmeroOleadaActual + 1}";
        if (!_gestorOleadas.HayMásOleadas) StartWaveButton.Content = "Sin más oleadas";

        UndoButton.IsEnabled = _historial.PuedeDeshacerse && !_juegoTerminado;
        UndoButton.Content = _historial.PuedeDeshacerse
            ? $"↶ Deshacer: {_historial.DescripcionPico}"
            : "↶ Deshacer última acción";

        foreach (var (tipo, btn) in _botonesDeTorre)
        {
            bool puedeComprar = _oro >= FabricaTorre.CostoDe(tipo);
            btn.IsEnabled = puedeComprar && !_juegoTerminado;
        }

        if (_torreSelecionada != null && UpgradePanel.Visibility == Visibility.Visible)
        {
            foreach (var btn in UpgradeOptionsPanel.Children.OfType<Button>())
            {
                if (btn.Tag is NodoMejora nodo) btn.IsEnabled = _oro >= nodo.Costo && !_juegoTerminado;
            }
        }
    }

    // ===================== DIBUJADO DEL CAMINO =====================

    private void DibujarCamino()
    {
        var exterior = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(0x40, 0x42, 0x55)),
            StrokeThickness = 34,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };
        var interior = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(0x2C, 0x2E, 0x3F)),
            StrokeThickness = 26,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round
        };
        foreach (var p in DatosMapa.PuntosControl)
        {
            exterior.Points.Add(p);
            interior.Points.Add(p);
        }
        Panel.SetZIndex(exterior, 0);
        Panel.SetZIndex(interior, 1);
        GameCanvas.Children.Add(exterior);
        GameCanvas.Children.Add(interior);

        var inicio = new Ellipse
        {
            Width = 18, Height = 18,
            Fill = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71)),
            Stroke = Brushes.Black, StrokeThickness = 1
        };
        Canvas.SetLeft(inicio, DatosMapa.PuntosControl[0].X - 9);
        Canvas.SetTop(inicio, DatosMapa.PuntosControl[0].Y - 9);
        Panel.SetZIndex(inicio, 2);
        GameCanvas.Children.Add(inicio);

        var fin = new Ellipse
        {
            Width = 18, Height = 18,
            Fill = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)),
            Stroke = Brushes.Black, StrokeThickness = 1
        };
        var ultimo = DatosMapa.PuntosControl[^1];
        Canvas.SetLeft(fin, ultimo.X - 9);
        Canvas.SetTop(fin, ultimo.Y - 9);
        Panel.SetZIndex(fin, 2);
        GameCanvas.Children.Add(fin);
    }

    // ===================== FIN DE PARTIDA =====================

    private void VerificarCondiciónDeVictoria()
    {
        if (_gestorOleadas.TodasLasOleadasTerminadas && _enemigos.Count == 0 && !_juegoTerminado)
            JuegoGanado();
    }

    private void JuegoGanado()
    {
        _juegoTerminado = true;
        _temporizador.Stop();
        EndTitle.Text = "¡VICTORIA!";
        EndTitle.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x44));
        EndSubtitle.Text = $"Sobreviviste con {_vidas} vidas y {_oro} oro.";
        EndOverlay.Visibility = Visibility.Visible;
    }

    private void JuegoPerdido()
    {
        _juegoTerminado = true;
        _temporizador.Stop();
        EndTitle.Text = "DERROTA";
        EndTitle.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x66, 0x66));
        EndSubtitle.Text = $"Alcanzaste la oleada {_gestorOleadas.NúmeroOleadaActual}.";
        EndOverlay.Visibility = Visibility.Visible;
    }

    private void BotónReiniciar_Click(object sender, RoutedEventArgs e)
    {
        GameCanvas.Children.Clear();
        _torres.Clear();
        _enemigos.Clear();
        _proyectiles.Clear();
        _historial.Limpiar();
        _oro = OroInicial;
        _vidas = VidasIniciales;
        _juegoTerminado = false;
        _tipoColocando = null;
        _torreSelecionada = null;
        _indicadorRangoSeleccionado = null;
        _rangoFantasma = null;
        _torreFantasma = null;
        _gestorOleadas = new GestorOleadas(BibliotecaOleadas.ConstruirTodasLasOleadas());
        EndOverlay.Visibility = Visibility.Collapsed;
        UpgradePanel.Visibility = Visibility.Collapsed;
        StatusLabel.Text = "Reiniciado. Pulsa \"Iniciar oleada\".";
        DibujarCamino();
        ActualizarResaltadoBotonesTorre();
        ActualizarEstadísticas();
        _ultimoTick = DateTime.Now;
        _temporizador.Start();
    }
}
