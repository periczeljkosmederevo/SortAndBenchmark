# High-Performance C# Sorting & Benchmarking Console Application

A high-performance C# console application and benchmarking suite designed to implement, test, and evaluate advanced multi-column sorting extension methods for traditional multi-dimensional arrays (`T[,]`) and jagged arrays (`T[][]`), powered by **BenchmarkDotNet**.

---

## 🚀 Vision & Scope

The primary goal of this open-source project is to provide a robust execution environment
and benchmark suite for testing high-performance, flexible, and memory-efficient sorting utilities
for C# developers working with complex tabular data structures. 

Unlike standard LINQ queries (`OrderBy` / `ThenBy`) which introduce object wrapping, 
delegate overhead, and heap allocations, the custom extension methods leverage native .NET sorting mechanisms (`IntroSort`),
specialized struct comparers, and reference pointer swapping to achieve maximum execution speed with minimal GC pressure.

This repository serves as a practical console application demonstrating these concepts, 
actively structured for easy expansion with new sorting algorithms and data structures over time.

---

## 🛠️ Core Features & Architecture

1. **Multi-Column Sorting Directives (`SortDirective` & `SortOrder`)**:
   * Define complex, hierarchical sorting rules across an arbitrary number of columns, 
   * supporting mixed ascending (`SortOrder.Ascending`) and descending (`SortOrder.Descending`) sequences.
2. **High-Performance Sorting Strategies**:
   * **Jagged QuickSort (`T[][]`)**: 
     * Swaps row reference pointers directly (`(array[j], array[i]) = (array[i], array[j])`) instead of copying individual cell elements.
   * **FastSort (`T[][]`)**: Combines .NET's native optimized `Array.Sort` (IntroSort) with a **zero-allocation struct comparer** (`JaggedRowComparer<T>`), 
   * enabling JIT devirtualization and aggressive method inlining (`[MethodImpl(MethodImplOptions.AggressiveInlining)]`).
   * **2D Array QuickSort (`T[,]`)**: Traditional in-place sorting adapted for multi-dimensional array structures.
3. **Built-in Benchmark Suite**:
   * Integrates **BenchmarkDotNet** inside the console entry point to continuously measure, profile, and compare execution times and memory allocations against standard LINQ baselines across dataset scales from 100 up to 1,000,000 rows (each containing 10 columns of string data).

---

## 📁 Project Structure

```text
Sort/
│
├── SortExtensions.cs         # Core extension methods: SortOrder, SortDirective, and high-performance algorithms
├── SortBenchmarks.cs         # BenchmarkDotNet profiling suite for performance regression testing
├── RandomListGenerator.cs    # Controlled test dataset generator with duplicate management (ties)
├── Program.cs                # Console entry point launching the BenchmarkRunner
└── README.md                 # Project documentation

```

---

## 📊 Example Usage
Integrating multi-column sorting into your own projects is straightforward using extension methods:

```csharp
    using Sort;

    // 1. Define multi-column sorting directives (e.g., Col 0 ASC, Col 1 DESC, Col 2 ASC)
    SortDirective[] directives =
    [
        new SortDirective(0, SortOrder.Ascending),
        new SortDirective(1, SortOrder.Descending),
        new SortDirective(2, SortOrder.Ascending)
    ];

    // 2. Example with a Jagged Array (T[][])
    string[][] jaggedTable = 
    [
        ["John", "Porter", "1234 Oak Street"],
        ["John", "Porter", "1891 3rd Street"],
        ["William", "Patterson", "4534 Virginia Street"]
    ];

    // Apply high-performance FastSort (IntroSort + zero-allocation struct comparer)
    jaggedTable.FastSortArray(directives);

    // Or use QuickSort variant on jagged array
    jaggedTable.QuickSortArray(directives);

    // 3. Example with a traditional 2D Array (T[,])
    string[,] table2D = new string[2, 3]
    {
        { "John", "Porter", "1234 Oak Street" },
        { "William", "Patterson", "4534 Virginia Street" }
    };

    table2D.QuickSortArray(directives);
```

---

## ⚙️ Running the Benchmarks
To evaluate sorting performance and run memory diagnostics on your machine:

1. Open the solution in Visual Studio 2026 or higher.

2. Set the build configuration to Release (essential for accurate JIT optimization and inlining).

3. Ensure the BenchmarkDotNet NuGet package is installed:

```text
    dotnet add package BenchmarkDotNet
```

4. Run the application (Ctrl + F5). BenchmarkDotNet will execute warmup cycles, run statistical iterations, and output detailed performance summaries.

---

## 🗺️ Roadmap & Future Extensions
[ ] Integration of parallel sorting (Parallel.Sort) for massive multi-million row datasets.

[ ] Support for span-based (ReadOnlySpan<T>) zero-copy sorting operations.

[ ] Additional sorting algorithms for performance comparison benchmarks.

---

## 📄 License
This project is licensed under the [CC0-1.0 license (Public Domain)](https://creativecommons.org/publicdomain/zero/1.0/).

To the extent possible under law, the author(s) have dedicated all copyright and related rights to this software to the public domain worldwide.

Feel free to use, modify, and distribute the code without any restrictions.

---

## 📧 Support & Contact
For technical inquiries, suggestions, or collaboration, please contact:

Name: Željko Perić

Email: periczeljkosmederevo@yahoo.com

---
