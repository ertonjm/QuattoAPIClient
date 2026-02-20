# QuattoAPIClient.Source

Componente principal do Quatto API Client para SSIS — responsável pela ingestão de dados via APIs REST.

---

## ✨ Funcionalidade

- Implementa o componente **CorporateApiSource** para Data Flow no SSIS.
- Suporte a paginação automática, retry, rate limiting, extração incremental (watermark), armazenamento de JSON bruto e telemetria.
- Utiliza helpers para HTTP, paginação, watermark, armazenamento bruto e mapeamento de esquema.

---

## 🗂 Estrutura
```
src/01_Source/
├── QuattoAPIClient.Source.csproj
├── Components/
│   └── CorporateApiSource.cs
├── Helpers/
│   ├── HttpHelper.cs
│   ├── PaginationEngine.cs
│   ├── WatermarkManager.cs
│   ├── RawStorageManager.cs
│   └── SchemaMapper.cs
└── Properties/
└── AssemblyInfo.cs

---

## 🛠 Dependências

- .NET Framework 4.7.2
- Referências SSIS: DTSPipelineWrap, PipelineHost, DTSRuntimeWrap, ManagedDTS

---

## 📋 Uso

1. Compile o projeto via Visual Studio 2019+.
2. Importe a DLL gerada no SSIS Toolbox.
3. Configure o componente no Data Flow, conectando ao Connection Manager e definindo propriedades conforme a documentação.

---

## 📚 Documentação

Consulte `../../docs/03_USAGE.md` para exemplos de configuração e uso.

---

## 🆘 Suporte

Problemas comuns:
- Erro de compilação: verifique referências SSIS no .csproj
- Componente não aparece: confira o deploy e permissões

Contato: Erton Miranda <erton.miranda@quatto.com.br>