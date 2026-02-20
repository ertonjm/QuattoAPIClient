# ?? BLOCO 5 - TESTES COMPLETOS (Guia Abrangente)

**Status**: ? PRONTO PARA INICIAR  
**Tempo Estimado**: 6-8 horas  
**Tarefas**: 5 (5.1 - 5.5)  
**Meta**: 95%+ Conformidade  

---

## ?? BLOCO 5 OVERVIEW

```
BLOCO 5.1: Release Build         (0.5-1h)
?? Compilar em Release
?? Validar performance
?? Verificar DLL

BLOCO 5.2: SSIS Types            (2-3h)
?? Testar com tipos reais
?? Validar métodos
?? Checkar integração

BLOCO 5.3: SSDT Wizard           (2-3h)
?? Testar no Visual Studio
?? Validar UI
?? Checkar persistência

BLOCO 5.4: Connection Manager    (1-2h)
?? Testar integração CM
?? Validar autenticação
?? Checkar carregamento

BLOCO 5.5: Exemplos Práticos     (1-2h)
?? Testar cenários reais
?? Validar fluxos completos
?? Documentar resultados

Total: 6-8 horas
```

---

## ?? BLOCO 5.1 - Release Build (0.5-1h)

### Objetivo
Compilar em Release mode e validar performance.

### Checklist

**1. Compilar em Release**
```powershell
# No Visual Studio:
Build ? Build Solution (Release configuration)

# Ou via CLI:
dotnet build -c Release
```

**2. Validar Output**
```
? 0 erros
? 0 warnings
? DLL gerado: bin\Release\ApiSourceWizard.dll
? PDB gerado: bin\Release\ApiSourceWizard.pdb
```

**3. Verificar Performance**
```
Build time:    < 30 segundos
DLL size:      < 100 KB
Memory usage:  Aceitável
```

**4. Validar Assembly**
```csharp
// Verify assembly info
// Product: Quatto API Client
// Version: Correto
// Culture: Neutro
```

### Success Criteria
```
? Release build compila sem erros
? DLL gerado com sucesso
? Tamanho razoável
? Performance aceitável
```

---

## ?? BLOCO 5.2 - SSIS Types (2-3h)

### Objetivo
Testar com tipos SSIS reais quando disponíveis.

### Opções

**Opção A: Com SSIS instalado**
```csharp
// Descomentar em ApiSourceWizard.cs:
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

// Testar com tipos reais
var pipeline = metadata as IDTSComponentMetaData100;
var connections = connections as Connections;
```

**Opção B: Sem SSIS (simulação)**
```csharp
// Mock objects para teste
public interface IMetadata { }
public interface IConnections { }

// Criar objetos mock
var mockMetadata = new Mock<IMetadata>();
var mockConnections = new Mock<IConnections>();
```

### Testes a Fazer

```
? Wizard instancia sem erro
? Carrega com objetos null (fallback)
? Métodos públicos acessíveis
? Propriedades inicializam
? Sem exceções
```

### Notas

```
Quando SSIS estiver disponível:
?? Descomentar tipos em GetPropertyValue()
?? Implementar lógica real
?? Testar com metadata real

Por enquanto:
?? Fallback para strings vazias
?? Defaults já funcionam
?? Pronto para quando SSIS chegar
```

---

## ?? BLOCO 5.3 - SSDT Wizard (2-3h)

### Objetivo
Testar wizard completamente no Visual Studio (SSDT).

### Pré-requisitos

```
? Visual Studio com SSIS instalado
? Integration Services Projects extension
? SSDT (SQL Server Data Tools)
? Nosso componente registrado
```

### Testes Funcionais

**1. Registrar Componente**
```
1. Copiar DLL para:
   C:\Program Files\Microsoft SQL Server\160\DTS\PipelineComponents\

2. Registrar no registry (se necessário)

3. Reiniciar Visual Studio
```

**2. Criar Package de Teste**
```
1. Integration Services Project novo
2. Novo SSIS Package
3. Data Flow Task
4. Add Data Flow Source (nosso componente)
```

**3. Testar UI do Wizard**
```
? Clica direito no componente ? Edit
? Wizard abre sem erro
? Todos os tabs visíveis
? Controles acessíveis
? Valores carregam
```

**4. Testar Funcionalidade**
```
? Consegue escrever valores
? Validação funciona
? Real-time feedback visual
? Tooltips aparecem
? GroupBoxes organizados
```

**5. Testar Persistência**
```
? Valores salvam
? Package salva sem erro
? Valores persistem ao reabrir
? Sem corrupção
```

### Checklist

```
Visual:
? UI profissional
? GroupBoxes bem definidas
? Cores ajudam organização
? Layout 900x700 adequado

Funcional:
? Todos os campos editáveis
? Validação bloqueia incorreto
? Avisos alertam configs extremas
? Real-time feedback visual

Usabilidade:
? Fácil de entender
? Tooltips ajudam
? Defaults sensatos
? Erros claros
```

---

## ?? BLOCO 5.4 - Connection Manager (1-2h)

### Objetivo
Testar integração com Connection Manager.

### Testes

**1. Connection Manager Carrega**
```
? ComboBox lista CMs disponíveis
? Pode selecionar CM
? Valor persiste
```

**2. Autenticação Funciona**
```
? CM valida credenciais
? Sem erros de conexão
? Dados carregam
```

**3. Integração Completa**
```
? Wizard ? CM ? Source
? Fluxo de dados completo
? Sem erros
```

### Checklist

```
Integration:
? Connection Manager integrado
? Valores sincronizados
? Sem conflitos
? Performance aceitável
```

---

## ?? BLOCO 5.5 - Exemplos Práticos (1-2h)

### Objetivo
Testar com cenários reais e documentar.

### Exemplos a Testar

**1. Paginação Offset**
```
Base URL:     https://api.gladium.com
Endpoint:     /v1/orders
Page Size:    500
Type:         Offset
Start Page:   1
Max Pages:    0 (sem limite)

Resultado:    ? Carrega dados com paginação
```

**2. Paginação Cursor**
```
Base URL:     https://api.exemplo.com
Endpoint:     /v2/items
Type:         Cursor
Start Page:   1
Max Pages:    100

Resultado:    ? Usa cursor para navegação
```

**3. Incremental com Watermark**
```
Enable Inc:   Checked
Watermark:    updated_at
System:       Gladium
Environment:  PRD

Resultado:    ? Só carrega novos registros
```

**4. Raw Storage**
```
Mode:         FileSystem
Target:       C:\Data\Raw
Compress:     Checked
Hash:         Checked

Resultado:    ? Armazena JSON comprimido e hasheado
```

**5. Retries e Rate Limit**
```
Max Retries:  5
Backoff:      Exponential
Base Delay:   1000ms
Rate Limit:   120 rpm

Resultado:    ? Retry inteligente com backoff
```

### Documentar

```
Para cada exemplo:
?? Configuração
?? Resultado esperado
?? Resultado obtido
?? Status (? / ?)
?? Notas
```

---

## ? CRITÉRIOS DE SUCESSO FASE 3

```
5.1 Release Build:
? Compila sem erros
? DLL gerado
? Performance OK

5.2 SSIS Types:
? Tipos validados
? Métodos funcionam
? Integração OK

5.3 SSDT Wizard:
? UI funciona
? Valores persistem
? Sem erros

5.4 Connection Manager:
? CM carrega
? Integração OK
? Autenticação OK

5.5 Exemplos:
? Todos funcionam
? Documentados
? Sem erros

Conformidade Final: 95%+
```

---

## ?? PRÓXIMO PASSO

### Começar BLOCO 5.1 - Release Build

```
Ação: Compilar em Release
      Validar output
      Checkar performance

Tempo: 0.5-1h
Status: PRONTO PARA INICIAR
```

---

**Versão**: 3.0.0  
**Status**: ?? BLOCO 5 PRONTO PARA INICIAR  
**Próximo**: BLOCO 5.1 (Release Build)

