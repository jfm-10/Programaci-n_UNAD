using System;
using System.Collections.Generic;

namespace CajeroAutomatico
{
    // Clase para manejar excepciones específicas del cajero
    public class CajeroException : Exception
    {
        public CajeroException(string message) : base(message) { }
    }

    // Clase para representar una cuenta en un banco
    public class Cuenta
    {
        public string NumeroCuenta { get; private set; }
        public double Saldo { get; private set; }
        public double TopeDiarioRetiros { get; private set; } = 2000000;
        public double RetiradoHoy { get; private set; } = 0;
        public int PuntosViveColombia { get; private set; }

        public Cuenta(string numeroCuenta, double saldoInicial, int puntosIniciales = 0)
        {
            NumeroCuenta = numeroCuenta;
            Saldo = saldoInicial;
            PuntosViveColombia = puntosIniciales;
        }

        public void Depositar(double monto)
        {
            Saldo += monto;
        }

        public void Retirar(double monto)
        {
            if (monto > Saldo)
                throw new CajeroException("Fondos insuficientes.");
            if (RetiradoHoy + monto > TopeDiarioRetiros)
                throw new CajeroException("Supera el tope diario de retiros.");
            Saldo -= monto;
            RetiradoHoy += monto;
        }

        public void Transferir(double monto, Cuenta cuentaDestino)
        {
            if (monto > Saldo)
                throw new CajeroException("Fondos insuficientes.");
            Saldo -= monto;
            cuentaDestino.Depositar(monto);
        }

        public void ConsultarSaldo()
        {
            Console.WriteLine($"Saldo actual: {Saldo:C}");
        }

        public void ConsultarPuntos()
        {
            Console.WriteLine($"Puntos ViveColombia: {PuntosViveColombia}");
        }

        public void CanjearPuntos(int puntos)
        {
            if (puntos > PuntosViveColombia)
                throw new CajeroException("Puntos insuficientes.");
            double monto = puntos * 7;
            PuntosViveColombia -= puntos;
            Saldo += monto;
            Console.WriteLine($"Canjeó {puntos} puntos por {monto:C}");
        }

        public void ReiniciarRetiroDiario()
        {
            RetiradoHoy = 0;
        }
    }

    // Clase que representa un cliente del banco
    public class Cliente
    {
        public string Nombre { get; private set; }
        public string Clave { get; private set; }
        public Cuenta Cuenta { get; private set; }

        public Cliente(string nombre, string clave, Cuenta cuenta)
        {
            Nombre = nombre;
            Clave = clave;
            Cuenta = cuenta;
        }

        public bool Autenticar(string clave)
        {
            return Clave == clave;
        }
    }

    // Clase que representa el cajero automático
    public class CajeroAutomatico
    {
        private Dictionary<string, Cliente> clientes;

        public CajeroAutomatico()
        {
            clientes = new Dictionary<string, Cliente>();
        }

        public void AgregarCliente(Cliente cliente)
        {
            if (!clientes.ContainsKey(cliente.Nombre))
            {
                clientes.Add(cliente.Nombre, cliente);
            }
        }

        public Cliente AutenticarCliente(string nombre, string clave)
        {
            if (clientes.ContainsKey(nombre) && clientes[nombre].Autenticar(clave))
            {
                return clientes[nombre];
            }
            throw new CajeroException("Autenticación fallida.");
        }

        public bool ClienteExiste(string nombre)
        {
            return clientes.ContainsKey(nombre);
        }

        public Cliente ObtenerCliente(string nombre)
        {
            if (clientes.ContainsKey(nombre))
            {
                return clientes[nombre];
            }
            throw new CajeroException("Cliente no encontrado.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Crea un cajero automático y crea un diccionario con los clientes
            CajeroAutomatico cajero = new CajeroAutomatico();
            
            Cuenta cuenta1 = new Cuenta("123456", 500000, 100);
            Cliente cliente1 = new Cliente("Juan", "1234", cuenta1);
            cajero.AgregarCliente(cliente1);

            Cuenta cuenta2 = new Cuenta("654321", 5000000, 200);
            Cliente cliente2 = new Cliente("Maria", "5678", cuenta2);
            cajero.AgregarCliente(cliente2);

            Cuenta cuenta3 = new Cuenta("789012", 850000, 150);
            Cliente cliente3 = new Cliente("Carlos", "9012", cuenta3);
            cajero.AgregarCliente(cliente3);

            try
            {
                Console.WriteLine("Ingrese su nombre de usuario:");
                string? nombre = Console.ReadLine();

                Console.WriteLine("Ingrese su clave:");
                string? clave = Console.ReadLine();

                if (nombre == null || clave == null)
                {
                    throw new CajeroException("Nombre de usuario o clave no pueden ser nulos.");
                }

                Cliente cliente = cajero.AutenticarCliente(nombre, clave);
                Console.WriteLine($"Bienvenido, {cliente.Nombre}");

                bool continuar = true;
                while (continuar)
                {
                    Console.WriteLine("Seleccione una operación:");
                    Console.WriteLine("1. Consultar saldo");
                    Console.WriteLine("2. Retirar dinero");
                    Console.WriteLine("3. Transferir dinero");
                    Console.WriteLine("4. Consultar puntos ViveColombia");
                    Console.WriteLine("5. Canjear puntos ViveColombia");
                    Console.WriteLine("6. Salir");

                    string? input = Console.ReadLine();
                    if (!int.TryParse(input, out int opcion))
                    {
                        Console.WriteLine("Opción no válida.");
                        continue;
                    }

                    switch (opcion)
                    {
                        case 1:
                            cliente.Cuenta.ConsultarSaldo();
                            break;
                        case 2:
                            Console.WriteLine("Ingrese el monto a retirar:");
                            input = Console.ReadLine();
                            if (input == null || !double.TryParse(input, out double montoRetiro))
                            {
                                Console.WriteLine("Monto no válido.");
                                break;
                            }
                            cliente.Cuenta.Retirar(montoRetiro);
                            Console.WriteLine("Retiro exitoso.");
                            break;
                        case 3:
                            Console.WriteLine("Ingrese el nombre del cliente destino:");
                            string? nombreDestino = Console.ReadLine();
                            Console.WriteLine("Ingrese el monto a transferir:");
                            input = Console.ReadLine();
                            if (nombreDestino == null || input == null || !double.TryParse(input, out double montoTransferencia))
                            {
                                Console.WriteLine("Datos no válidos.");
                                break;
                            }

                            if (cajero.ClienteExiste(nombreDestino))
                            {
                                Cliente destinatario = cajero.ObtenerCliente(nombreDestino);
                                cliente.Cuenta.Transferir(montoTransferencia, destinatario.Cuenta);
                                Console.WriteLine("Transferencia exitosa.");
                            }
                            else
                            {
                                Console.WriteLine("Cliente destino no encontrado.");
                            }
                            break;
                        case 4:
                            cliente.Cuenta.ConsultarPuntos();
                            break;
                        case 5:
                            Console.WriteLine("Ingrese la cantidad de puntos a canjear:");
                            input = Console.ReadLine();
                            if (input == null || !int.TryParse(input, out int puntos))
                            {
                                Console.WriteLine("Cantidad de puntos no válida.");
                                break;
                            }
                            cliente.Cuenta.CanjearPuntos(puntos);
                            break;
                        case 6:
                            continuar = false;
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
            }
            catch (CajeroException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }
    }
}


