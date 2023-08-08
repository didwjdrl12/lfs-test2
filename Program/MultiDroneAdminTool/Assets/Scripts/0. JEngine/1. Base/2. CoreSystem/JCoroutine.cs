using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCoroutines
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JCoroutines
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable/Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] routines

        private List<IEnumerator> routines = new List<IEnumerator>();


        #endregion

        #region [Property] Count / Running
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int Count
        {
            get { return routines.Count; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Running
        {
            get { return routines.Count > 0; }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 업데이트
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Update] 코루틴 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            for (int i = 0; i < routines.Count; i++)
            {
                if (routines[i].Current is IEnumerator)
                    if (MoveNext((IEnumerator)routines[i].Current))
                        continue;
                if (!routines[i].MoveNext())
                    routines.RemoveAt(i--);
            }
        }
        #endregion

        #region [Update] [구현] MoveNext
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator)
                if (MoveNext((IEnumerator)routine.Current))
                    return true;
            return routine.MoveNext();
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Init
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Coroutine] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Start(IEnumerator routine)
        {
            routines.Add(routine);
        }
        #endregion


        #region [Coroutine] StopAll
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopAll()
        {
            routines.Clear();
        }
        #endregion

        #region [Coroutine] Pause
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static IEnumerator Pause(float time)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < time)
                yield return 0;
        }
        #endregion        
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JCoroutinesHelper (Sample)
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JCoroutinesHelper
    {
        #region [Sample] [Variable] poem
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //"Death" by John Donne
        const string poem = "\"Death\" by John Donne\n\n" +
                            "Death be not proud, though some have called thee\n" +
                            "Mighty and dreadfull, for, thou art not so,\n" +
                            "For, those, whom thou think'st, thou dost overthrow,\n" +
                            "Die not, poore death, nor yet canst thou kill me.\n" +
                            "From rest and sleepe, which but thy pictures bee,\n" +
                            "Much pleasure, then from thee, much more must flow,\n" +
                            "And soonest our best men with thee doe goe,\n" +
                            "Rest of their bones, and soules deliverie.\n" +
                            "Thou art slave to Fate, Chance, kings, and desperate men,\n" +
                            "And dost with poyson, warre, and sicknesse dwell,\n" +
                            "And poppie, or charmes can make us sleepe as well,\n" +
                            "And better then thy stroake; why swell'st thou then;\n" +
                            "One short sleepe past, wee wake eternally,\n" +
                            "And death shall be no more; death, thou shalt die.";
        #endregion

        #region [Sample] ReadPoem
        //------------------------------------------------------------------------------------------------------------------------------------------------------               
        //static void Main(string[] args)
        //{
        //    var coroutines = new JCoroutines();
        //    coroutines.Start(ReadPoem(poem));
        //    while (coroutines.Running)
        //        coroutines.Update();
        //}

        static IEnumerator ReadPoem(string poem)
        {
            //Read the poem letter by letter
            foreach (var letter in poem)
            {
                Console.Write(letter);
                switch (letter)
                {
                    //Pause for punctuation
                    case ',':
                    case ';':
                        yield return Pause(0.5f);
                        break;

                    //Long pause for full-stop
                    case '.':
                        yield return Pause(1);
                        break;

                    //Short pause for anything else
                    default:
                        yield return Pause(0.05f);
                        break;
                }
            }

            //Wait for user input to close
            Console.WriteLine("\nPress any key to exit");
            Console.ReadLine();
        }

        public static IEnumerator Pause(float time)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < time)
                yield return 0;
        }
        #endregion

    }

}
