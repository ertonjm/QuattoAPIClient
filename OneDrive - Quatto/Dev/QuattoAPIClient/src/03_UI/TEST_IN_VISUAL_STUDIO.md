# 🧪 Guia de Teste em Visual Studio - SSIS v17.100

## 📋 Pré-requisitos

✅ Visual Studio 2022 Enterprise (já instalado)
✅ SQL Server 2022 com SSIS (já instalado)
✅ .NET Framework 4.7.2 Developer Pack

---

## 🚀 PASSO 1: Abrir a Solução

1. Abra **Visual Studio 2022**
2. Clique em **File → Open → Folder** 
3. Navegue até: `C:\Users\ErtonLuisdeJesusMira\OneDrive - Quatto\Dev\QuattoAPIClient\src`
4. Ou abra diretamente o arquivo `.sln` se existir

**Alternativa rápida:**
```powershell
start "" "C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\devenv.exe" "C:\Users\ErtonLuisdeJesusMira\OneDrive - Quatto\Dev\QuattoAPIClient\src\QuattoAPIClient.sln"
```

---

## 🔧 PASSO 2: Clean Solution

1. **Solution Explorer** → Clique com botão direito na Solução
2. Selecione: **Clean Solution**
3. Aguarde completar (removerá todos os binários antigos)

**Status esperado:** "Clean succeeded" na barra de status

---

## 🔨 PASSO 3: Rebuild All

1. **Build** → **Rebuild Solution** (ou **Ctrl+Shift+B**)
2. Deixe compilar completamente

**Tempo esperado:** ~10-15 segundos

---

## ✅ PASSO 4: Verificar Resultado

### ✅ Se compilar com SUCESSO:

```
========== Rebuild All: 2 succeeded, 0 failed, 0 skipped ==========
Build completed at HH:MM:SS and took X,XXX seconds
```

**O que fazer:**
- ✅ Parabéns! Configuração está correta
- ✅ Ambos projetos (UI + ConnectionManager) compilaram
- ✅ Próximo passo: Implementar logging estruturado

---

### ⚠️ Se tiver WARNINGS (aceitável):

```
warning MSB3277: There was a conflict between "Microsoft.SqlServer..."
warning MSB3270: There was a mismatch between processor architecture...
```

**Isso é normal:** App.config binding redirects vão resolver em runtime

---

### 🔴 Se tiver ERROS:

#### Erro: "Could not locate assembly 'Microsoft.SqlServer.ManagedDTS'"

```
C:\path\QuattoAPIClient.ConnectionManager.csproj
error CS0246: The type or namespace name 'ConnectionManagerBase' could not be found
```

**Solução:**
1. Verifique se SQL Server 2022 SSIS está instalado
2. Check: Control Panel → Programs → Programs and Features
3. Procure por: "SQL Server 2022 Integration Services"
4. Se não achar: Instale via SQL Server 2022 Installer

#### Erro: "The type or namespace name 'Dts' does not exist"

```
error CS0234: The type or namespace name 'Dts' does not exist in the namespace 'Microsoft.SqlServer'
```

**Solução:**
1. Project → Properties → Target framework
2. Confirme: **.NET Framework 4.7.2**
3. Clean → Rebuild

#### Erro: "cannot be marshaled by the runtime marshaler"

```
warning MSB3305: Processing COM reference...
```

**Solução:** (Já aplicada)
```xml
<NoWarn>$(NoWarn);MSB3305;MSB3277</NoWarn>
```

---

## 📊 CHECKLIST DE VALIDAÇÃO

Depois de compilar com sucesso, verifique:

- [ ] **UI Project** compilou sem erros
- [ ] **ConnectionManager Project** compilou sem erros
- [ ] **Output** gerado em:
  - `src\03_UI\bin\Debug\net472\QuattoAPIClient.UI.dll`
  - `src\02_ConnectionManager\bin\Debug\net472\QuattoAPIClient.ConnectionManager.dll`
- [ ] **XML Documentation** foi gerado (`.xml` files)

---

## 🔍 VERIFICAÇÕES ADICIONAIS

### 1. Verificar References
```
Solution Explorer 
→ QuattoAPIClient.UI (ou ConnectionManager)
→ Dependencies → Assemblies
```

Procure por:
- ✅ `Microsoft.SqlServer.DTSPipelineWrap (17.100.0.0)`
- ✅ `Microsoft.SqlServer.DTSRuntimeWrap (17.100.0.0)`
- ✅ `Microsoft.SqlServer.ManagedDTS (17.100.0.0)`

### 2. Verificar app.config
```
Solution Explorer
→ QuattoAPIClient.ConnectionManager
→ app.config
```

Confirme:
- ✅ Binding redirects apontam para `17.100.0.0`
- ✅ Não há valores de `15.0.0.0` mais antigos

### 3. Verificar .csproj
```
Solution Explorer
→ Projeto
→ Propriedades (Properties)
→ Build
```

Confirme:
- ✅ Platform target: **x64**
- ✅ Target framework: **.NET Framework 4.7.2**

---

## 🎯 Resultado Esperado

```
QuattoAPIClient.UI → Build succeeded
QuattoAPIClient.ConnectionManager → Build succeeded
```

Se isso aparecer, você está **100% pronto** para desenvolver com SSIS v17.100!

---

## 📞 Se tiver problemas:

1. **Copie a saída completa** do Output window
2. Procure por `error CS` ou `error MSB`
3. Google the error message
4. Verifique se SQL Server 2022 SSIS está no GAC:

```powershell
gacutil -l Microsoft.SqlServer.ManagedDTS
```

Se não encontrar, instale via SQL Server 2022 Installer → Modify → Integration Services

---

**✅ Próximo passo quando compilar com sucesso:**
- Phase 2: Adicionar Logging Estruturado
- Phase 3: Criar Testes Unitários
- Phase 4: Documentação Completa

