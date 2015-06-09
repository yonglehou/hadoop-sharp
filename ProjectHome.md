[Hadoop](http://hadoop.apache.org/core/) is an Apache project for distributed parallel computing using the [MapReduce](http://en.wikipedia.org/wiki/MapReduce) computational framework.

While Hadoop is written in Java, hadoop-sharp enables developers to use any CLR (.NET/Mono) supported programming language (C#, VB.NET, F#, [IronPython](http://www.ironpython.com/), [IronRuby](http://www.ironruby.net/)) for their [MapReduce](http://en.wikipedia.org/wiki/MapReduce) computations.

The current version uses the Hadoop Pipes interface and is modeled after
the Hadoop Pipes C++ API, passing around byte arrays. The idea is to evolve
the API to use all the fancy features of the CLR such as generics,
lambda functions etc. and eventually LINQ.

(The current version is NOT ready for end-users. This Google Code project is a temporary home in order to facilitate rapid collaborative development before being submitted to the upstream Hadoop repository)