<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='1']">
	<configuration version="2">
	  <xsl:apply-templates mode ="Migrate1To2"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate1To2">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate1To2"/>
  </xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="identity">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="identity"/>
	</xsl:copy>
  </xsl:template>


  <!-- ============= actual changing starts here ================ -->
  <xsl:template match="configuration[@version!='1']">
	<configuration>
	  <xsl:attribute name="version"><xsl:value-of select="current()/@version"/></xsl:attribute>
	  <xsl:apply-templates  mode ="identity"></xsl:apply-templates>
	</configuration>
  </xsl:template>



  <!-- ============= change tasks for use definition instead of gloss ================ -->

  <xsl:template match="*[text() = 'SenseGloss']"   mode="Migrate1To2">
	<xsl:element name="{name(.)}">
	  <xsl:text>definition</xsl:text>
	</xsl:element>
  </xsl:template>

  <xsl:template match="readOnly[text() = 'SenseGloss, ExampleSentence']"   mode="Migrate1To2">
	<readOnly>definition, ExampleSentence</readOnly>
  </xsl:template>

  <xsl:template  match="task[@id='AddMeanings']/filter/field"    mode="Migrate1To2">
	<field>definition</field>
  </xsl:template>

  <xsl:template  match="showfields[text()='SenseGloss' or text()='gloss']"    mode="Migrate1To2">
	<showfields>definition</showfields>
  </xsl:template>

  <!-- ============= change the id for SenseGloss to gloss ================ -->

  <xsl:template match="fields/field/fieldName[text() = 'SenseGloss']"   mode="Migrate1To2">
	<fieldName>gloss</fieldName>
  </xsl:template>

</xsl:stylesheet>