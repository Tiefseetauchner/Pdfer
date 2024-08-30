using System.Collections.Generic;

namespace Pdfer;

public record XRefTable(
  List<XRefEntry> XRefEntries);
