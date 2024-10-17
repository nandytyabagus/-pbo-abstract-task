using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.JalankanSimulasi();
    }
}

public abstract class Robot
{
    public string nama;
    public int energi;
    public int armor;
    public int serangan;

    protected Robot(string nama, int energi, int armor, int serangan)
    {
        this.nama = nama;
        this.energi = energi;
        this.armor = armor;
        this.serangan = serangan;
    }

    public virtual void Serang(Robot target)
    {
        int damage = Math.Max(0, serangan - target.armor);
        target.energi -= damage;
        Console.WriteLine($"{nama} MENYERANG {target.nama}\nMENGHASILKAN {damage} KERUSAKAN!");
    }

    public abstract void GunakanKemampuan(IKemampuan kemampuan, Robot target);

    public virtual void CetakInformasi()
    {
        Console.WriteLine($"Nama: {nama}, Energi: {energi}, Armor: {armor}, Serangan: {serangan}");
    }

    public virtual void PulihkanEnergi(int jumlah)
    {
        energi += jumlah;
        Console.WriteLine($"{nama} MEMULIHKAN {jumlah} ENERGI.");
    }

    public bool IsAlive()
    {
        return energi > 0;
    }
}

public interface IKemampuan
{
    string Nama { get; }
    void Gunakan(Robot pengguna, Robot target);
    bool DapatDigunakan();
    void ResetCooldown();
}

public class Perbaikan : IKemampuan
{
    public string Nama => "PERBAIKAN";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int jumlahPerbaikan = 20;
        pengguna.PulihkanEnergi(jumlahPerbaikan);
        cooldown = 3;
        Console.WriteLine($"{pengguna.nama} PERBAIKAN DIGUNAKAN\nMEMULIHKAN {jumlahPerbaikan} ENERGI!");
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class SeranganListrik : IKemampuan
{
    public string Nama => "SERANGAN LISTRIK";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int damage = 20;
        target.energi -= damage;
        Console.WriteLine($"{pengguna.nama} MENGGUNAKAN SERANGAN PLASMA PADA {target.nama}\nMENGHASILKAN {damage} KERUSAKAN!");
        cooldown = 2;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class SeranganPlasma : IKemampuan
{
    public string Nama => "SERANGAN PLASMA";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int damage = 40;
        target.energi -= damage;
        Console.WriteLine($"{pengguna.nama} MENGGUNAKAN SERANGAN PLASMA PADA {target.nama}\nMENGHASILKAN {damage} KERUSAKAN MENEMBUS ARMOR!");
        cooldown = 4;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class PertahananSuper : IKemampuan
{
    public string Nama => "PERTAHANAN SUPER";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        pengguna.armor += 10;
        Console.WriteLine($"{pengguna.nama} PERTAHANAN SUPER AKTIF, ARMOR MENINGKAT 10!");
        cooldown = 3;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class RobotBiasa : Robot
{
    private List<IKemampuan> kemampuan;

    public RobotBiasa(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
    {
        kemampuan = new List<IKemampuan>
        {
            new Perbaikan(),
            new SeranganListrik()
        };
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        if (kemampuan.DapatDigunakan())
        {
            kemampuan.Gunakan(this, target);
        }
        else
        {
            Console.WriteLine($"{nama} TIDAK DAPAT MENGGUNAKAN {kemampuan.Nama} TIME COOLDOWN.");
        }
    }

    public List<IKemampuan> DapatkanKemampuan()
    {
        return kemampuan;
    }
}

public class RobotKhusus : Robot
{
    private List<IKemampuan> kemampuan;

    public RobotKhusus(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
    {
        kemampuan = new List<IKemampuan>
        {
            new SeranganPlasma(),
            new PertahananSuper()
        };
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        if (kemampuan.DapatDigunakan())
        {
            kemampuan.Gunakan(this, target);
        }
        else
        {
            Console.WriteLine($"{nama} TIDAK DAPAT MENGGUNAKAN {kemampuan.Nama} TIME COOLDOWN.");
        }
    }

    public List<IKemampuan> DapatkanKemampuan()
    {
        return kemampuan;
    }
}

public class BosRobot : Robot
{
    public int pertahanan;

    public BosRobot(string nama, int energi, int pertahanan, int serangan) : base(nama, energi, pertahanan / 2, serangan)
    {
        this.pertahanan = pertahanan;
    }

    public void Diserang(Robot penyerang, bool ignoreArmor = false)
    {
        int damage = ignoreArmor ? penyerang.serangan : Math.Max(0, penyerang.serangan - pertahanan);
        energi -= damage;
        Console.WriteLine($"{nama} MENERIMA SERANGAN {penyerang.nama}\nDAMAGE DITERIMA {damage} KERUSAKAN!");
    }

    public void Mati()
    {
        Console.WriteLine($"{nama} TELAH DIKALAHKAN");
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        Console.WriteLine($"{nama} TIDAK MEMILIKI KEMAMPUAN KHUSUS.");
    }
}

public class Game
{
    private List<Robot> robots;
    private BosRobot bosRobot;

    public Game()
    {
        robots = new List<Robot>
        {
            new RobotBiasa("N00B1", 200, 90, 20),
            new RobotKhusus("GG3WP", 230, 75, 35)
        };
        bosRobot = new BosRobot("MEGAMAN", 500, 100, 60);
    }

    public void JalankanSimulasi()
    {
        Console.WriteLine("=====================================================");
        Console.WriteLine("WAR ROBOT DAR DAR DAR");
        Console.WriteLine("PERTARUNGAN DIMULAI");
        Console.WriteLine("=====================================================\n");

        int giliran = 1;
        while (bosRobot.energi > 0 && robots.Exists(r => r.energi > 0))
        {
            Console.WriteLine($"=== GILIRAN {giliran} ===");

            foreach (var robot in robots)
            {
                if (!robot.IsAlive()) continue;

                Console.WriteLine($"\ngiliran:{robot.nama}");
                robot.CetakInformasi();

                Console.WriteLine("\nPILIH AKSI:");
                Console.WriteLine("1. SERANG BOS ROBOT");
                Console.WriteLine("2. GUNAKAN KEMAMPUAN");

                int pilihan;
                while (!int.TryParse(Console.ReadLine(), out pilihan) || pilihan < 1 || pilihan > 2)
                {
                    Console.WriteLine("!!!PILIHAN TIDAK VALID!!!\nSilakan pilih 1 atau 2.");
                }

                if (pilihan == 1)
                {
                    robot.Serang(bosRobot);
                }
                else
                {
                    List<IKemampuan> kemampuanList = (robot is RobotBiasa robotBiasa) ? robotBiasa.DapatkanKemampuan() : ((RobotKhusus)robot).DapatkanKemampuan();
                    Console.WriteLine("\nPILIH KEMAMPUAN:");
                    for (int i = 0; i < kemampuanList.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {kemampuanList[i].Nama}");
                    }

                    int pilihanKemampuan;
                    while (!int.TryParse(Console.ReadLine(), out pilihanKemampuan) || pilihanKemampuan < 1 || pilihanKemampuan > kemampuanList.Count)
                    {
                        Console.WriteLine($"!!!PILIHAN TIDAK VALID!!!\nSILAKAN PILIH 1 SAMPAI {kemampuanList.Count}.");
                    }

                    robot.GunakanKemampuan(kemampuanList[pilihanKemampuan - 1], bosRobot);
                }

                if (bosRobot.IsAlive())
                {
                    robot.energi -= Math.Max(0, bosRobot.serangan - robot.armor);
                    Console.WriteLine($"{bosRobot.nama} MENYERANG BALIK {robot.nama}\nLMENYEBABKAN {Math.Max(0, bosRobot.serangan - robot.armor)} KERUSAKAN!");
                }

                robot.PulihkanEnergi(5);

                if (robot is RobotBiasa rb)
                {
                    rb.DapatkanKemampuan().ForEach(k => k.ResetCooldown());
                }
                else if (robot is RobotKhusus rk)
                {
                    rk.DapatkanKemampuan().ForEach(k => k.ResetCooldown());
                }
            }

            Console.WriteLine("\nSTATUS : ");
            bosRobot.CetakInformasi();

            giliran++;
            Console.WriteLine("\nTEKAN ENTER UNTUK MELANJUTKAN");
            Console.ReadLine();
        }

        if (bosRobot.energi <= 0)
        {
            Console.WriteLine("!!!YOU WIN!!!");
        }
        else
        {
            Console.WriteLine("!!!GAME OVER!!!");
        }
    }
}
