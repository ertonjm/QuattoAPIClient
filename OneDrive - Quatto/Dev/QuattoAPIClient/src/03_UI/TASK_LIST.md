# ?? TASK LIST - Finalização do Módulo UI

**Projeto**: Quatto API Client v1.0  
**Módulo**: src/03_UI (CorporateApiSourceUI)  
**Status Atual**: 62.67% conforme  
**Target**: 100% conforme  
**Data Criação**: 2024-01-XX  
**Estimativa Total**: 31-42 horas

---

## ?? TAREFAS CRÍTICAS (Bloqueantes) - BLOCO 1 & 2

### BLOCO 1: UI Visual do Wizard ? 8-10 horas

#### 1.1 - Implementar InitializeComponent()
- [x] Criar TabControl com 5 abas
- [x] Setup básico do formulário
- [x] Layout inicial
- **Status**: ? CONCLUÍDO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1 (concluído)
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: 
  ```
  Arquivo: src/03_UI/Forms/ApiSourceWizard.cs
  Linhas: 50-150
  ? Compilado sem erros
  ? 5 tabs criados (estrutura)
  ? Botões OK, Cancel, Apply implementados
  ```

#### 1.2 - Criar Tab "General"
- [x] ComboBox para Connection Manager
- [x] TextBox para Base URL
- [x] TextBox para Endpoint
- [x] NumericUpDown para PageSize
- [x] Labels e layout
- **Status**: ? CONCLUÍDO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1.5 (concluído)
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6 - Create GeneralTab
- **Notas**: 
  ```
  ? ComboBox para Connection Manager implementado
  ? TextBox para Base URL implementado
  ? TextBox para Endpoint implementado
  ? NumericUpDown para PageSize implementado
  ? Labels com layout correto (alinhamento)
  ? Compilado sem erros
  ? 4 controles + 4 labels criados
  ```

#### 1.3 - Criar Tab "Pagination"
- [x] ComboBox para tipo paginação (Offset, Cursor, Link-based, None)
- [x] NumericUpDown para StartPage
- [x] NumericUpDown para MaxPages
- [x] Validações específicas por tipo
- **Status**: ? CONCLUÍDO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1.2 (concluído)
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: 
  ```
  ? ComboBox para tipo paginação implementado (4 tipos)
  ? NumericUpDown para StartPage implementado
  ? NumericUpDown para MaxPages implementado (0 = sem limite)
  ? Label informativo adicionado
  ? Compilado sem erros
  ? 3 controles + 4 labels criados
  ```

#### 1.4 - Criar Tab "Incremental"
- [ ] CheckBox para EnableIncremental
- [ ] TextBox para WatermarkColumn
- [ ] TextBox para SourceSystem
- [ ] ComboBox para Environment (DEV/HML/PRD)
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1-1.5
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: Importante para cargas incrementais

#### 1.5 - Criar Tab "Storage"
- [ ] ComboBox para RawStoreMode (None/SqlVarbinary/FileSystem)
- [ ] TextBox para RawStoreTarget
- [ ] CheckBox para CompressRawJson
- [ ] CheckBox para HashRawJson
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1-1.5
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: Controla armazenamento de JSON bruto

#### 1.6 - Criar Tab "Advanced"
- [ ] NumericUpDown para MaxRetries
- [ ] ComboBox para BackoffMode
- [ ] NumericUpDown para BaseDelayMs
- [ ] NumericUpDown para RateLimitRPM
- [ ] NumericUpDown para TimeoutSeconds
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1.5-2
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: Configurações avançadas de retry e rate limit

#### 1.7 - Adicionar Botões (OK, Cancel, Apply)
- [x] Implementar eventos click
- [x] DialogResult correto
- [x] Validação ao clicar OK
- [x] Button layout
- **Status**: ? CONCLUÍDO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 0.5 (concluído)
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 6
- **Notas**: 
  ```
  ? OK, Cancel, Apply buttons criados
  ? Layout correto no painel inferior
  ? Click handlers conectados
  ```

---

### BLOCO 2: Persistência de Dados ? 4-6 horas

#### 2.1 - Implementar LoadCurrentValues()
- [ ] Carregar cada propriedade do metadata
- [ ] Popular cada controle na UI
- [ ] Try-catch com mensagens
- [ ] Usar método helper GetPropertyValue()
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1.5-2
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 3
- **Notas**: 
  ```csharp
  Carrega valores salvos previamente
  Called quando wizard abre
  ```

#### 2.2 - Implementar SaveValues()
- [ ] Validar propriedades (método próprio)
- [ ] Salvar cada controle no metadata
- [ ] Usar método helper SetPropertyValue()
- [ ] Disparar FireComponentMetaDataModifiedEvent()
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 1.5-2
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 4
- **Notas**: Called ao clicar OK ou Apply

#### 2.3 - Implementar GetPropertyValue(string name)
- [ ] Buscar em CustomPropertyCollection
- [ ] Buscar em ComponentProperties
- [ ] Null-safe, com return padrão
- [ ] Tratamento de exceções
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 0.5-1
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 5
- **Notas**: Método helper para leitura de propriedades

#### 2.4 - Implementar SetPropertyValue(string name, string value)
- [ ] Validar se propriedade existe
- [ ] Fazer cast correto
- [ ] Tratamento de tipos especiais
- [ ] Log de erros
- **Status**: ? TODO
- **Prioridade**: ?? CRÍTICA
- **Horas**: 0.5-1
- **Referência**: IMPLEMENTATION_GUIDE.md Passo 5
- **Notas**: Método helper para escrita de propriedades

---

## ?? TAREFAS IMPORTANTES (Alta Prioridade) - BLOCO 3 & 4

### BLOCO 3: Validação ? 4-5 horas

#### 3.1 - Implementar ValidateProperties()
- [ ] Validar URL (não vazio + bem formada)
- [ ] Validar PageSize (> 0)
- [ ] Validar WatermarkColumn se incremental ativo
- [ ] Validar RetryCount (0-10)
- [ ] Retornar bool com motivo
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 1.5-2
- **Notas**: Called antes de SaveValues()

#### 3.2 - Adicionar validação real-time
- [ ] Validar ao sair do campo (Leave event)
- [ ] Validar base URL imediatamente
- [ ] Validar numbers (PageSize, etc)
- [ ] Feedback visual (cor vermelha em erro)
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 1-1.5
- **Notas**: Melhor UX, feedback ao usuário

#### 3.3 - Adicionar mensagens de erro específicas
- [ ] "Base URL inválida: deve começar com https://"
- [ ] "PageSize inválido: deve ser entre 1 e 10000"
- [ ] "WatermarkColumn obrigatório quando incremental ativo"
- [ ] Show em MessageBox com title específico
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 1-1.5
- **Notas**: Mensagens claras para usuário

#### 3.4 - Implementar avisos (warnings)
- [ ] Avisar se RateLimitRPM > 1000 (muito alto)
- [ ] Avisar se PageSize > 5000 (pode ser lento)
- [ ] Avisar se TimeoutSeconds < 30 (pode ser curto)
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 0.5-1
- **Notas**: Alertas não-bloqueantes

---

### BLOCO 4: Melhorias de UX ? 3-4 horas

#### 4.1 - Adicionar Labels descritivos
- [ ] Cada controle com label claro
- [ ] Tooltips com help text
- [ ] Width/Height consistent
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 1-1.5
- **Notas**: Usabilidade do wizard

#### 4.2 - Adicionar valores padrão
- [ ] PageSize: 500
- [ ] MaxRetries: 5
- [ ] TimeoutSeconds: 100
- [ ] BaseDelayMs: 1000
- [ ] RateLimitRPM: 120
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 0.5-1
- **Notas**: Acelera configuração

#### 4.3 - Melhorar layout
- [ ] Padding consistent (10px)
- [ ] Alinhamento das colunas
- [ ] Height automático dos tabs
- [ ] Redimensionável
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 1-1.5
- **Notas**: Aparência profissional

#### 4.4 - Adicionar seções com GroupBox
- [ ] Agrupar campos relacionados
- [ ] Visual mais organizado
- [ ] Melhor legibilidade
- **Status**: ? TODO
- **Prioridade**: ?? ALTA
- **Horas**: 0.5-1
- **Notas**: Opcional, mas melhora UX

---

## ?? TAREFAS IMPORTANTES (Média Prioridade) - BLOCO 5, 6, 7

### BLOCO 5: Testes de Integração ? 6-8 horas

#### 5.1 - Testar compilação completa
- [ ] dotnet build -c Release
- [ ] Esperar 0 erros, 0 warnings
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 0.5
- **Notas**: Verificação básica

#### 5.2 - Testar com tipos SSIS reais
- [ ] Copiar arquivos do OneDrive primeiro
- [ ] Compilar com assembly SSIS real
- [ ] Validar imports funcionam
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 1-1.5
- **Notas**: Requer SSIS 2019+ instalado

#### 5.3 - Testar wizard em SSDT
- [ ] Registrar componente na toolbox
- [ ] Arrasta na data flow
- [ ] Clica direito > Edit
- [ ] Wizard abre sem erro
- [ ] Carrega valores atuais
- [ ] Permite editar
- [ ] Salva valores
- [ ] Valores persistem após reabrir
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 2-3
- **Notas**: Teste completo do ciclo

#### 5.4 - Testar com Connection Manager
- [ ] Criar API Connection Manager
- [ ] Carregar no combo do wizard
- [ ] Validar autenticação funciona
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 1-1.5
- **Notas**: Integração com módulo 02_

#### 5.5 - Testar com exemplos
- [ ] Usar SchemaMapping_Gladium.json
- [ ] Usar SchemaMapping_PortalSESC.json
- [ ] Testar paginação offset
- [ ] Testar incremental com watermark
- [ ] Testar raw storage
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 1.5-2
- **Notas**: Testes com casos reais

---

### BLOCO 6: Documentação do UI ? 2-3 horas

#### 6.1 - Atualizar IMPLEMENTATION_GUIDE.md
- [ ] Adicionar screenshots do wizard
- [ ] Documentar cada tab
- [ ] Adicionar exemplos de configuração
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 1
- **Notas**: Documentação visual

#### 6.2 - Atualizar README.md do módulo UI
- [ ] Adicionar seção "Usando o Wizard"
- [ ] Adicionar prints do wizard funcionando
- [ ] Adicionar exemplos de usuários finais
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 1
- **Notas**: Guide para end-users

#### 6.3 - Criar guia rápido de UI
- [ ] Passo-a-passo visual
- [ ] Atalhos de teclado
- [ ] Dicas e truques
- **Status**: ? TODO
- **Prioridade**: ?? MÉDIA
- **Horas**: 0.5-1
- **Notas**: Quick reference

---

### BLOCO 7: Recursos Avançados ? 4-6 horas (Nice to Have)

#### 7.1 - Adicionar botão "Test Connection"
- [ ] Validar connection manager
- [ ] Testar HTTP request
- [ ] Mostrar resultado
- **Status**: ? TODO
- **Prioridade**: ?? BAIXA
- **Horas**: 1.5-2
- **Notas**: Feature nice-to-have

#### 7.2 - Adicionar preview de configuração
- [ ] Mostrar URL construída
- [ ] Mostrar headers
- [ ] Mostrar query string
- **Status**: ? TODO
- **Prioridade**: ?? BAIXA
- **Horas**: 1-1.5
- **Notas**: Visualização da config

#### 7.3 - Adicionar auto-complete
- [ ] Sugestões de endpoints conhecidas
- [ ] Cache de valores anteriores
- [ ] Inteligência de tipos
- **Status**: ? TODO
- **Prioridade**: ?? BAIXA
- **Horas**: 1-1.5
- **Notas**: UX enhancement

#### 7.4 - Adicionar schema mapping visual
- [ ] Button para editar schema JSON
- [ ] Visual tree para navegar
- [ ] Validation de path JSON
- **Status**: ? TODO
- **Prioridade**: ?? BAIXA
- **Horas**: 1-1.5
- **Notas**: Recurso avançado

---

## ?? RESUMO POR FASE

### ?? FASE 1: CRÍTICO (2-3 dias) - 12-16 horas
```
BLOCO 1: UI Visual (8-10h)
BLOCO 2: Persistência (4-6h)
?????????????????????????
Total: 12-16 horas
Resultado: Componente utilizável ?
```

**Tarefas**:
- [ ] 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7
- [ ] 2.1, 2.2, 2.3, 2.4

---

### ?? FASE 2: IMPORTANTE (2-3 dias) - 13-17 horas
```
BLOCO 3: Validação (4-5h)
BLOCO 4: UX (3-4h)
BLOCO 5: Testes (6-8h)
????????????????????????
Total: 13-17 horas
Resultado: Componente robusto ?
```

**Tarefas**:
- [ ] 3.1, 3.2, 3.3, 3.4
- [ ] 4.1, 4.2, 4.3, 4.4
- [ ] 5.1, 5.2, 5.3, 5.4, 5.5

---

### ?? FASE 3: DOCUMENTAÇÃO (1 dia) - 2-3 horas
```
BLOCO 6: Docs (2-3h)
????????????????????
Total: 2-3 horas
Resultado: Documentação atualizada ?
```

**Tarefas**:
- [ ] 6.1, 6.2, 6.3

---

### ?? FASE 4: ENHANCEMENTS (1-2 dias) - 4-6 horas (Opcional)
```
BLOCO 7: Avançados (4-6h)
?????????????????????????
Total: 4-6 horas
Resultado: Features extras ?
```

**Tarefas**:
- [ ] 7.1, 7.2, 7.3, 7.4

---

## ?? TIMELINE RECOMENDADA

| Fase | Duração | Data Início | Data Fim | Status |
|------|---------|------------|----------|--------|
| **Fase 1** | 2-3 dias | DD/MM/YYYY | DD/MM/YYYY | ? TODO |
| **Fase 2** | 2-3 dias | DD/MM/YYYY | DD/MM/YYYY | ? TODO |
| **Fase 3** | 1 dia | DD/MM/YYYY | DD/MM/YYYY | ? TODO |
| **Fase 4** | 1-2 dias | DD/MM/YYYY | DD/MM/YYYY | ? OPCIONAL |
| **TOTAL** | 6-9 dias | | | |

---

## ?? PROGRESSO

```
Conformidade Esperada por Fase:
?? Atual: 62.67% ??????????????????
?? Após Fase 1: 85% ??????????????????
?? Após Fase 2: 95% ?????????????????
?? Após Fase 3: 100% ??????????????????
?? Após Fase 4: 100% (Enhanced)
```

---

## ? CHECKLIST FINAL

- [ ] Fase 1: UI Visual + Persistência funcionando
- [ ] Fase 2: Validação + UX + Testes passando
- [ ] Fase 3: Documentação atualizada
- [ ] Fase 4: Recursos avançados (opcional)
- [ ] Compilação sem erros: ?
- [ ] Compilação sem warnings: ?
- [ ] Componente aparece na toolbox: ?
- [ ] Wizard abre e salva: ?
- [ ] Integração com Connection Manager: ?
- [ ] Integração com Database: ?
- [ ] Documentação 100%: ?
- [ ] Testes passando: ?

---

## ?? NOTAS E OBSERVAÇÕES

### Pré-requisitos
- SQL Server 2019+ com SSIS instalado
- Visual Studio 2019+ com SSDT
- .NET Framework 4.7.2+
- Arquivos do OneDrive copiados (BLOCO 1 & 2 pendentes)

### Dependências Entre Tarefas
```
1.1 (Init) 
  ??? 1.2, 1.3, 1.4, 1.5, 1.6 (Tabs)
       ??? 1.7 (Buttons)
            ??? 2.1, 2.2 (Persist)
                 ??? 3.1, 3.2, 3.3, 3.4 (Validation)
                      ??? 5.3, 5.4, 5.5 (Testes)
                           ??? 6.1, 6.2, 6.3 (Docs)
```

### Recursos Disponíveis
- ? IMPLEMENTATION_GUIDE.md - Código pronto
- ? TROUBLESHOOTING.md - Respostas para problemas
- ? README.md - Contexto do projeto
- ? Exemplos JSON - Casos de uso reais

---

## ?? PRÓXIMO PASSO

**Ação Imediata**: Iniciar **BLOCO 1.1**

1. Abrir `src/03_UI/Forms/ApiSourceWizard.cs`
2. Implementar `InitializeComponent()`
3. Criar TabControl básico
4. Tempo estimado: 1 hora
5. Referência: IMPLEMENTATION_GUIDE.md Passo 6

**Comando para start**:
```powershell
cd src/03_UI
# Abrir ApiSourceWizard.cs no Visual Studio
# Começar em InitializeComponent()
```

---

## ?? CONTATO & SUPORTE

- **Desenvolvedor**: Erton Miranda
- **Email**: erton.miranda@quatto.com.br
- **Documentação**: Consulte IMPLEMENTATION_GUIDE.md
- **Problemas**: Verifique TROUBLESHOOTING.md

---

**Versão**: 1.0.0  
**Status**: ? EM EXECUÇÃO  
**Última Atualização**: 2024-01-XX  
**Próxima Revisão**: Após Fase 1

