using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.SendSMSServices
{
    public class RsaEncryptor
    {
        //public void pkcs1pad2(string s, int n)
        //{
        //    if (n < s.Length + 11)
        //    {

        //    }
        //    int[] ba = new int[256];
        //    var i = s.Length - 1;
        //    while (i >= 0 && n > 0)
        //    {
        //        var c = (int)s.ToCharArray()[i--];
        //        if (c < 128)
        //        {
        //            ba[--n] = c;
        //        }
        //        else if ((c > 127) && (c < 2048))
        //        {
        //            ba[--n] = (c & 63) | 128;
        //            ba[--n] = (c >> 6) | 192;
        //        }
        //        else
        //        {
        //            ba[--n] = (c & 63) | 128;
        //            ba[--n] = ((c >> 6) & 63) | 128;
        //            ba[--n] = (c >> 12) | 224;
        //        }
        //    }
        //    ba[--n] = 0;
        //    int[] x;

        //    while (n > 2)
        //    {
        //        x[0] = 0;
        //        while (x[0] == 0) rng.nextBytes(x);
        //        ba[--n] = x[0];
        //    }

        //}

        //int rng_get_bytes(int[256] ba)
        //{
        //    var i;
        //    for (i = 0; i < ba.length; ++i) ba[i] = rng_get_byte();
        //}


        //private int[] rng_pool;
        //private int rng_pptr;
        //int rng_psize = 256;
        //void rng_seed_time()
        //{
        //    rng_seed_int(DateTime.Now.Millisecond);
        //}

        //void rng_seed_int(int x)
        //{
        //    rng_pool[rng_pptr++] ^= x & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 8) & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 16) & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 24) & 255;
        //    if (rng_pptr >= rng_psize) rng_pptr -= rng_psize;
        //}


        //int rng_get_bytes(int[] ba)
        //{
        //    var i;
        //    for (i = 0; i < ba.Length; ++i) ba[i] = rng_get_byte();
        //}
        //int rng_get_byte()
        //{
        //    if (rng_state == null)
        //    {
        //        rng_seed_time();
        //        rng_state = prng_newstate();
        //        rng_state.init(rng_pool);
        //        for (rng_pptr = 0; rng_pptr < rng_pool.length; ++rng_pptr)
        //            rng_pool[rng_pptr] = 0;
        //        rng_pptr = 0;
        //    }
        //    return rng_state.next();
        //}
        //int rng_seed_time()
        //{
        //    rng_seed_int(DateTime.Now);
        //}

        //private int[] rng_pool;
        //private int rng_pptr;
        //private int rng_psize = 256;
        //private Arcfour rng_state;

        //void rng_seed_int(int x)
        //{
        //    rng_pool[rng_pptr++] ^= x & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 8) & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 16) & 255;
        //    rng_pool[rng_pptr++] ^= (x >> 24) & 255;
        //    if (rng_pptr >= rng_psize) rng_pptr -= rng_psize;
        //}
        //Arcfour prng_newstate()
        //{
        //    return new Arcfour();
        //}
        //public class Arcfour
        //{
        //    public int i = 0;
        //    public int j = 0;
        //    public int[] S = new int[256];
        //}
    }
}
