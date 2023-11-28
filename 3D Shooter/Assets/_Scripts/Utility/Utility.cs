using System;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        //prng的名字含义：伪随机数生成器 pseudo-random number generator
        Random prng = new Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {

            //随机一个索引
            int randomIndex = prng.Next(i, array.Length);

            //交换
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
