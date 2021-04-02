module Demo
{
    // ["clr:generic:List"]
    // sequence<MyClass> MyClassList;
    sequence<byte> ByteArray;

    interface Printer
    {
        void printString(string s);
        string getLibraryContent();
        
        ["amd"] void uploadFile(ByteArray file);
        ["amd"] void uploadFileChunk(string filename, int offset, ByteArray file);
    }
}