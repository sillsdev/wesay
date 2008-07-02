<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='2']">
	<configuration version="3">
	  <xsl:apply-templates mode ="Migrate2To3"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="identity">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="identity"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="configuration[@version!='2']">
	<configuration>
	  <xsl:attribute name="version">
		<xsl:value-of select="current()/@version"/>
	  </xsl:attribute>
	  <xsl:apply-templates  mode ="identity"></xsl:apply-templates>
	</configuration>
  </xsl:template>


  <!-- ============= actual changing starts here ================ -->

  <!-- ============= change to new dashboard class ============== -->

  <xsl:template match="task[@id='Dashboard']" mode="Migrate2To3">
	<task id="Dashboard" class="WeSay.CommonTools.Dash" assembly="CommonTools">
	  <xsl:apply-templates select="@visible|node()"  mode="Migrate2To3"/>
	</task>
  </xsl:template>

  <!-- ================== remove Actions task =================== -->

  <xsl:template match="task[@id='Actions']"  mode="Migrate2To3"/>

  <!-- ============= add long labels where appropriate ========== -->

  <xsl:template match="task[@id='Dictionary']/label"   mode="Migrate2To3">
	<label UseInConstructor="false">Dictionary</label>
	<longLabel UseInConstructor="false">Dictionary Browse &amp; Edit</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='AddMeanings']/label"   mode="Migrate2To3">
	<label>Meanings</label>
	<longLabel>Add Meanings</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='AddPartOfSpeech']/label"   mode="Migrate2To3">
	<label>Parts of Speech</label>
	<longLabel>Add Parts of Speech</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='AddExamples']/label"   mode="Migrate2To3">
	<label>Example Sentences</label>
	<longLabel>Add Example Sentences</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='AddBaseForms']/label"   mode="Migrate2To3">
	<label>Base Forms</label>
	<longLabel>Add Base Forms</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='Gather Words (D)']/label"   mode="Migrate2To3">
	<label>Duerksen Word List</label>
	<longLabel>Gather Words Using Duerksen's List</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='Gather Words (PNG)']/label"   mode="Migrate2To3">
	<label>PNG Word List</label>
	<longLabel>Gather Words Using PNG List</longLabel>
  </xsl:template>

  <xsl:template match="task[@id='Gather Words By Domain']/label"   mode="Migrate2To3">
	<label>Semantic Domains</label>
	<longLabel>Gather Words By Semantic Domain</longLabel>
  </xsl:template>

  <!-- ============= add count labels where appropriate ========= -->

  <xsl:template match="task[@id='AddMeanings']/description"   mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
	<remainingCountText>Entries without meanings:</remainingCountText>
	<referenceCountText>Total entries:</referenceCountText>
  </xsl:template>

  <xsl:template match="task[@id='AddPartOfSpeech']/description"   mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
	<remainingCountText>Entries without parts of speech:</remainingCountText>
	<referenceCountText>Total entries:</referenceCountText>
  </xsl:template>

  <xsl:template match="task[@id='AddExamples']/description"   mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
	<remainingCountText>Entries without examples:</remainingCountText>
	<referenceCountText>Total entries:</referenceCountText>
  </xsl:template>

  <xsl:template match="task[@id='AddBaseForms']/description"   mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
	<remainingCountText>Entries without base forms:</remainingCountText>
	<referenceCountText>Total entries:</referenceCountText>
  </xsl:template>

  <xsl:template match="task[@id='Gather Words By Domain']/description"   mode="Migrate2To3">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate2To3"/>
	</xsl:copy>
	<remainingCountText>Domains without words:</remainingCountText>
	<referenceCountText>Total domains:</referenceCountText>
  </xsl:template>

</xsl:stylesheet>