This fork
=========

This is a fork of the [protobuf-net library](http://code.google.com/p/protobuf-net/), written by Marc Gravell, with a small number of fixes we have applied to fix some bugs and improve performance, shared here in the hope that others will find them useful.

protobuf-net
============

protobuf-net is a binary serialization engine, along similar lines to BinaryFormatter
but using the "protocol buffers" [PB] specification as laid out by Google.

The core protobuf-net library is a single dll; other projects are presented for
testing and example purposes.

Many PB implementations start with a .proto text file (describing the types), with
the developer running a command-line tool to generate code in their target
language. Deliberately, protobuf-net started the other way around: it is designed
to work with existing .NET types at runtime. This allows you to use your existing
data objects, but still with lots of optimisations to keep things very fast.

There is a C# code generation version (courtesy of Jon Skeet), and a proposed future
extension is to allow generation of protobuf-net classes from a .proto file.
