# CalculatorApp

Calculator application built with **WPF** using the **MVVM** (Model-View-ViewModel) design pattern. Includes both **Standard Mode** and a dedicated **Programmer Mode** for working in binary, decimal, hexadecimal, and octal numeral systems.

## 📸 Screenshots
![image](https://github.com/user-attachments/assets/551cc949-d227-4bb5-8853-aa7c9bfc0465) ![image](https://github.com/user-attachments/assets/6b19ae03-6fb6-4ec9-a28f-c4ba7a4193b4)



## ✨ Features

- ✅ Basic arithmetic operations (+, −, ×, ÷, %)
- 🔢 Programmer Mode: convert and display between Binary, Octal, Decimal, Hexadecimal
- 🧠 Memory operations: Store, Recall, Add to memory, Subtract, Clear individual or all
- 📖 Calculation history
- ↩️ Undo (Backspace), CE, C support
- 📐 Unary operations: √, x², 1/x, ±
- 🧠 Memory list visibility toggle
- 🧑‍💻 Keyboard support for fast input
- 🌗 UI hover effects and theme consistency

## 🧱 Technologies Used

- C#
- WPF (.NET)
- MVVM design pattern
- XAML for UI
- ICommand and RelayCommand pattern
- DataBinding and `INotifyPropertyChanged`

## 🗂 Project Structure

CalculatorApp/

├── App.xaml  
├── App.xaml.cs  
  └── → Application entry point
   
├── MainWindow.xaml  
├── MainWindow.xaml.cs  
   └── → Standard calculator interface  
   
├── ProgrammerWindow.xaml  
├── ProgrammerWindow.xaml.cs  
   └── → Programmer mode interface 

├── ViewModels/  
   └── CalculatorViewModel.cs  
       └── → Main ViewModel containing all calculator logic  
       
├── Converters/  
   └── BoolToVisibilityConverter.cs  
       └── → Handles boolean to visibility binding  
       
├── RelayCommand.cs  
   └── → Command pattern implementation
