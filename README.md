# WPF ListView Data Leak Fixer

## Overview

This project addresses a common issue in WPF applications where memory leaks occur due to improper management of `ListView` bindings and data collections. The application demonstrates a solution for handling data efficiently in a WPF `ListView` to prevent memory leaks and optimize performance, especially in scenarios involving large datasets.

## Features

- **Weak Observable Collection**: Utilizes a custom `WeakObservableCollection<T>` to manage data, ensuring that references do not lead to memory leaks.
- **Automatic Garbage Collection**: Incorporates garbage collection triggers during scroll events to free up memory.
- **Dynamic Data Loading**: Supports adding, removing, and reloading large datasets dynamically.
- **MVVM Architecture**: Implements the Model-View-ViewModel (MVVM) pattern for separation of concerns and better maintainability.
- **Command Binding**: Uses `ICommand` for clean and reactive UI interactions.

## How It Works

The application fixes data leak issues by:
1. Using a custom `WeakObservableCollection<T>` to manage data bindings. This collection tracks items with weak references, allowing the garbage collector to reclaim memory when needed.
2. Triggering garbage collection explicitly during `ListView` scroll events to handle memory-intensive operations.
3. Providing buttons to add, remove, and reload data for testing and simulating real-world scenarios.

## Requirements

- .NET 6.0 or later
- Visual Studio 2022 or any compatible IDE
- A basic understanding of WPF and MVVM

## Usage

1. Clone or download this repository.
2. Open the project in Visual Studio.
3. Build and run the solution.
4. Interact with the application using the following:
   - **Scroll the ListView**: Observe the memory management during scrolling.
   - **Add Items**: Use the "Add" button to add random strings to the list.
   - **Remove Items**: Use the "Remove" button to delete selected items.
   - **Clear Items**: Use the "Remove All" button to clear the collection.
   - **Reload Data**: Use the "Reload" button to repopulate the list with new data.

## Additional informations

If you try to load 100000 string (or any other data type, since WeakObservableCollection uses generics) you wouldn't even be able to load the app once.
