using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Models
{
  public class Test
  {
    interface IVendorTransDetails
    {
      void getVendorID();
    }
    interface IClaimsTracker
    {
      void getSeqID();
    }
    class ClaimsMaster
    {
      string getDCNNO()
      {
        return "PC20100308A00005";
      }
    }


    abstract class Abstract : ClaimsMaster, IClaimsTracker, IVendorTransDetails
    {
      //Here we should implement modifiers oterwise it throws complie-time error
      public void getVendorID()
      {
        int s = new int();
        s = 001;
        Console.Write(s);
      }

      public void getSeqID()
      {
        int SeqID = new int();
        SeqID = 001;
        Console.Write(SeqID);
      }
    }
  }
}
