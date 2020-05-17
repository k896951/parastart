// bgwait
//
// Copyright (C) 2020 Hideki Gotoh ( k896951 )
//
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
//
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace parastart
{
    enum retcodeType
    {
        allways
       , anysafe
       , allsafe
    }

    class Program
    {

        static int Main(string[] args)
        {
            parsargs p = new parsargs(args);
            Process[] pa;
            Task[] ta;

            if (0 == p.commands.Length)
            {
                help();
                Environment.Exit(0);
            }

            pa = new Process[p.commands.Length];
            ta = new Task[p.commands.Length];

            for (int i = 0; i < p.commands.Length; i++)
            {
                int ii = i;

                pa[ii] = new Process();
                ta[ii] = new Task(() =>
                {
                    pa[ii].StartInfo.FileName = "CMD.EXE";
                    pa[ii].StartInfo.Arguments = "/C " + "\"" + p.commands[ii] + "\"";
                    pa[ii].StartInfo.UseShellExecute = false;
                    pa[ii].StartInfo.CreateNoWindow = true;
                    pa[ii].StartInfo.ErrorDialog = true;
                    pa[ii].Start();
                    if (0 != p.wait)
                    {
                        pa[ii].WaitForExit(p.wait);
                    }
                    else
                    {
                        pa[ii].WaitForExit();
                    }
                });

                ta[ii].Start();
            }

            Task.WaitAll(ta);

            int ret = 16;
            int safecount = 0;

            for (int i = 0; i < pa.Length; i++)
            {
                if (0 == pa[i].ExitCode) safecount++;
            }

            if ((p.runmode == retcodeType.allsafe) && (pa.Length == safecount))
            {
                ret = 0;
            }
            if ((p.runmode == retcodeType.anysafe) && (0 != safecount))
            {
                ret = 0;
            }

            return ret;
        }

        static void help()
        {
            String pn = "parastart.exe";
            Console.WriteLine("{0} - Parallel execution of multiple programs.", pn);
            Console.WriteLine("\nusage: {0} [-any|-all|-zero] [-wait n] cmd1 cmd2 cmd3 ...\n", pn);
            Console.WriteLine("-zero   : Default. Return code is always zero.");
            Console.WriteLine("-any    : If one or more of the program has been successfully, the return code is zero.");
            Console.WriteLine("-all    : If all of the program has been successfully, the return code is zero.");
            Console.WriteLine("-wait n : Set n seconds to the execution limit time of each program. * Default is unlimited.");
            Console.WriteLine("cmd1 cmd2 cmd3... : List of *.bat or *.cmd program that execution in parallel.");
        }
    }

    class parsargs
    {
        String confFile = "";
        retcodeType retType = retcodeType.allways;
        int waitTime = 0;
        String[] cmds;

        Regex optReg = new Regex(@"^-[a-zA-Z]+");
        String dummyfile = "dummyfile";

        public retcodeType runmode
        {
            get
            {
                return retType;
            }
        }
        public String config
        {
            get
            {
                return confFile;
            }
        }
        public String[] commands
        {
            get
            {
                return cmds;
            }
        }
        public int wait
        {
            get
            {
                return waitTime;
            }
        }
        public parsargs(string[] args)
        {
            Boolean swcmdflg = false;
            ArrayList ag = new ArrayList();

            for (int i = 0; i < args.Length; i++)
            {
                if ((true == optReg.IsMatch(args[i])) && (false == swcmdflg))
                {
                    switch (args[i].ToLower())
                    {
                        case "-zero":
                            retType = retcodeType.allways;
                            break;

                        case "-any":
                            retType = retcodeType.anysafe;
                            break;

                        case "-all":
                            retType = retcodeType.allsafe;
                            break;

                        case "-f":
                            if ((i + 1) < args.Length)
                            {
                                i++;
                                confFile = args[i];
                            }
                            break;

                        case "-wait":
                        case "-limit":
                            if ((i + 1) < args.Length)
                            {
                                i++;
                                try
                                {
                                    waitTime = 1000 * int.Parse(args[i]);
                                }
                                catch (Exception)
                                {
                                    waitTime = 0;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    swcmdflg = true;
                    if (dummyfile != args[i])
                    {
                        ag.Add(args[i]);
                    }
                }
            }

            cmds = ag.ToArray(typeof(String)) as String[];
        }
    }
}